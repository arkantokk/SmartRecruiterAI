using System.Text; // Required for StringBuilder
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

    private const string MailServer = "imap.gmail.com";
    private const int MailPort = 993;

    public EmailBackgroundWorker(
        IServiceScopeFactory scopeFactory, 
        ILogger<EmailBackgroundWorker> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mailUser = _configuration["EmailSettings:Email"];
        var mailPassword = _configuration["EmailSettings:Password"];

        if (string.IsNullOrEmpty(mailUser) || string.IsNullOrEmpty(mailPassword))
        {
            _logger.LogError("❌ Email credentials missing! Run 'dotnet user-secrets set' to configure.");
            return;
        }

        _logger.LogInformation($"📧 Email Worker Started via IMAP for {mailUser}...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a new client for every iteration to ensure stability
                using (var client = new ImapClient())
                {
                    client.Timeout = 10000; // 10 seconds timeout
                    await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);
                    await client.AuthenticateAsync(mailUser, mailPassword, stoppingToken);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    // Search for UNSEEN emails only
                    var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);

                    if (uids.Count > 0)
                    {
                        _logger.LogInformation($"🔥 Found {uids.Count} new emails! Processing...");

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                            var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                            var vacancyRepository = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>();

                            foreach (var uid in uids)
                            {
                                var message = await inbox.GetMessageAsync(uid, stoppingToken);
                                var subject = message.Subject.Trim();
                                
                                _logger.LogInformation($"📩 Checking email Subject: '{subject}'");

                                // 1. Match Email Subject with Job Vacancy Title
                                var vacancy = await vacancyRepository.GetByTitleAsync(subject);

                                if (vacancy != null)
                                {
                                    _logger.LogInformation($"✅ Matched Vacancy: {vacancy.Title} (ID: {vacancy.Id})");

                                    // 2. AGGREGATE CONTEXT (Body + Attachments)
                                    // We use StringBuilder to combine everything into one large text for AI
                                    var fullDossier = new StringBuilder();

                                    // A) Append Email Body (Cover Letter)
                                    fullDossier.AppendLine("=== EMAIL BODY / COVER LETTER ===");
                                    fullDossier.AppendLine(message.TextBody ?? "(No text body provided)");
                                    fullDossier.AppendLine("\n=== ATTACHMENTS ===");

                                    bool hasContent = false;

                                    // B) Loop through ALL attachments
                                    foreach (var attachment in message.Attachments)
                                    {
                                        if (attachment is MimePart part && part.FileName.ToLower().EndsWith(".pdf"))
                                        {
                                            _logger.LogInformation($"📎 Reading PDF: {part.FileName}");
                                            
                                            // Process PDF in memory
                                            using (var stream = new MemoryStream())
                                            {
                                                await part.Content.DecodeToAsync(stream, stoppingToken);
                                                stream.Position = 0; // Rewind stream
                                                
                                                var pdfText = await parsingService.ExtractTextAsync(stream);
                                                
                                                // C) Append PDF text to dossier
                                                fullDossier.AppendLine($"--- DOCUMENT: {part.FileName} ---");
                                                fullDossier.AppendLine(pdfText);
                                                fullDossier.AppendLine("--------------------------------");
                                                
                                                hasContent = true;
                                            }
                                        }
                                    }

                                    // 3. Register Candidate if we found any PDF or significant text
                                    if (hasContent || fullDossier.Length > 50)
                                    {
                                        var request = new CreateCandidateRequest
                                        {
                                            FirstName = message.From.Mailboxes.FirstOrDefault()?.Name ?? "Unknown",
                                            LastName = "(From Email)",
                                            Email = message.From.Mailboxes.FirstOrDefault()?.Address ?? "no-email",
                                            JobVacancyId = vacancy.Id,
                                            ResumeText = fullDossier.ToString() // Passing the full aggregated text
                                        };

                                        var newId = await candidateService.RegisterCandidateAsync(request);
                                        _logger.LogInformation($"🎉 SUCCESS! Candidate created via Email: {newId}");
                                    }
                                    else
                                    {
                                        _logger.LogWarning("⚠️ Email matched vacancy but contained no PDF or readable text.");
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning($"⚠️ Skipped: No vacancy found for subject '{subject}'");
                                }

                                // 4. Mark email as READ (Seen) so we don't process it again
                                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                            }
                        }
                    }
                    
                    await client.DisconnectAsync(true, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"🔥 Email Worker Error: {ex.Message}. Retrying in 15s...");
            }

            // Wait 15 seconds before next check
            _logger.LogInformation("💤 Waiting 15s for next check...");
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}