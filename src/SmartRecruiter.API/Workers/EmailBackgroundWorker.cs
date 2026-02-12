using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.API.Workers;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IStorageService _storageService;
    
    // Constant IMAP settings for Gmail
    private const string MailServer = "imap.gmail.com";
    private const int MailPort = 993;

    public EmailBackgroundWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailBackgroundWorker> logger,
        IConfiguration configuration,
        IStorageService storageService
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
        _storageService = storageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 1. INITIAL DIAGNOSTICS - Visible in Azure Log Stream
        Console.WriteLine($"\n[STAMP: {DateTime.Now:HH:mm:ss}] === WORKER BOOTSTRAP ===");
        
        // Try to fetch credentials (works with both ":" and "__" in Azure)
        var mailUser = _configuration["EmailSettings:Email"];
        var mailPassword = _configuration["EmailSettings:Password"];

        Console.WriteLine($"[CONFIG] Target User: {mailUser ?? "NULL!"}");
        Console.WriteLine($"[CONFIG] Password detected: {(string.IsNullOrEmpty(mailPassword) ? "NO" : "YES")}");

        if (string.IsNullOrEmpty(mailUser) || string.IsNullOrEmpty(mailPassword))
        {
            _logger.LogError("CRITICAL: Email credentials missing. Ensure 'EmailSettings__Email' is set in Azure.");
            Console.WriteLine("[ERROR] Missing credentials. BackgroundWorker will stop.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    // Increase timeout for cloud stability
                    client.Timeout = 20000; 
                    
                    Console.WriteLine($"\n[IMAP] Connecting to {MailServer}:{MailPort}...");
                    await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);
                    
                    Console.WriteLine($"[IMAP] Authenticating {mailUser}...");
                    await client.AuthenticateAsync(mailUser, mailPassword, stoppingToken);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    // SEARCH: Look for UNREAD (Unseen) emails only
                    var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);
                    Console.WriteLine($"[IMAP] Inbox check complete. Found {uids.Count} unread emails.");

                    if (uids.Count > 0)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                            var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                            var vacancyRepository = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>();

                            foreach (var uid in uids)
                            {
                                var message = await inbox.GetMessageAsync(uid, stoppingToken);
                                var subject = message.Subject?.Trim() ?? "";
                                
                                Console.WriteLine($"[PROCESS] Examining email: \"{subject}\" from {message.From}");

                                // VACANCY MATCHING - Must be an EXACT match with the Subject
                                var vacancy = await vacancyRepository.GetByTitleAsync(subject);
                                
                                if (vacancy == null)
                                {
                                    Console.WriteLine($"[SKIP] No vacancy found matching title: \"{subject}\". Check DB or Subject line!");
                                    continue; 
                                }

                                Console.WriteLine($"[MATCH] Found Vacancy: {vacancy.Title} (ID: {vacancy.Id})");

                                var fullDossier = new StringBuilder();
                                fullDossier.AppendLine("=== EMAIL BODY ===");
                                fullDossier.AppendLine(message.TextBody ?? string.Empty);
                                fullDossier.AppendLine("\n=== ATTACHMENTS ===");

                                bool hasPdf = false;
                                string? resumeUrl = null;

                                foreach (var attachment in message.Attachments)
                                {
                                    if (attachment is MimePart part && part.FileName.ToLower().EndsWith(".pdf"))
                                    {
                                        using (var stream = new MemoryStream())
                                        {
                                            await part.Content.DecodeToAsync(stream, stoppingToken);
                                            stream.Position = 0;
                                            
                                            Console.WriteLine($"[PARSING] Extracting text from: {part.FileName}");
                                            var text = await parsingService.ExtractTextAsync(stream);
                                            
                                            fullDossier.AppendLine($"--- FILE: {part.FileName} ---");
                                            fullDossier.AppendLine(text);
                                            
                                            stream.Position = 0;
                                            resumeUrl = await _storageService.UploadAsync(stream, part.FileName, "application/pdf");
                                            hasPdf = true;
                                        }
                                    }
                                }

                                // If processed, register the candidate
                                if (hasPdf || fullDossier.Length > 100)
                                {
                                    var request = new CreateCandidateRequest
                                    {
                                        FirstName = message.From.Mailboxes.FirstOrDefault()?.Name ?? "Unknown",
                                        LastName = "Candidate",
                                        Email = message.From.Mailboxes.FirstOrDefault()?.Address ?? "no-email",
                                        JobVacancyId = vacancy.Id,
                                        ResumeText = fullDossier.ToString(),
                                        ResumeUrl = resumeUrl
                                    };

                                    await candidateService.RegisterCandidateAsync(request);
                                    Console.WriteLine($"[SUCCESS] Registered candidate: {request.Email} for {vacancy.Title}");
                                }

                                // Mark as read to avoid double processing
                                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                            }
                        }
                    }

                    await client.DisconnectAsync(true, stoppingToken);
                    Console.WriteLine("[IMAP] Session ended. Sleeping for 30s...");
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"[FATAL ERROR] {DateTime.Now:HH:mm:ss}: {ex.Message}";
                Console.WriteLine(errorMsg);
                _logger.LogError(ex, "Background worker error occurred.");
            }

            // Polling interval
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}