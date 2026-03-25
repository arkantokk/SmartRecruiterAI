using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security; // REQUIRED FOR OAUTH2 (SaslMechanismOAuth2)
using MimeKit;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.API.Workers;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundWorker> _logger;
    private readonly IStorageService _storageService;
    
    // Constant IMAP settings for Gmail
    private const string MailServer = "imap.gmail.com";
    private const int MailPort = 993;

    // We removed IConfiguration because we no longer read passwords from appsettings.json
    public EmailBackgroundWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailBackgroundWorker> logger,
        IStorageService storageService
    )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _storageService = storageService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"\n[STAMP: {DateTime.Now:HH:mm:ss}] === WORKER BOOTSTRAP ===");

        // The main loop that runs forever (every 30 seconds)
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. CREATE A NEW SCOPE
                // This is required to access our Scoped services (like DB Repositories) 
                // from this Singleton background worker safely.
                using (var scope = _scopeFactory.CreateScope())
                {
                    // 2. RESOLVE SERVICES FROM THE SCOPE
                    var emailIntegrationRepository = scope.ServiceProvider.GetRequiredService<IEmailIntegrationRepository>();
                    var gmailAuthService = scope.ServiceProvider.GetRequiredService<IGmailAuthService>();
                    var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                    var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                    var vacancyRepository = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>();

                    // 3. FETCH ALL INTEGRATIONS (MULTI-TENANT)
                    // We ask the database: give me the list of ALL recruiters who connected their Gmail.
                    var integrations = await emailIntegrationRepository.GetAllIntegrationAsync();

                    // 4. LOOP THROUGH EVERY CONNECTED EMAIL ACCOUNT
                    foreach (var integration in integrations)
                    {
                        Console.WriteLine($"\n[ACCOUNT] Processing account: {integration.ConnectedEmail}");
                        
                        // Create a temporary variable to hold the current integration data
                        var currentIntegration = integration; 

                        // 5. CHECK TOKEN EXPIRATION
                        // If the token is already expired OR will expire in less than 2 minutes...
                        if (currentIntegration.AccessTokenExpiresAt < DateTimeOffset.UtcNow.AddMinutes(2))
                        {
                            Console.WriteLine($"[AUTH] Token for {currentIntegration.ConnectedEmail} is expiring. Refreshing...");
                            
                            // Call our service to fetch a new token from Google and save it to the DB
                            await gmailAuthService.RefreshIntegrationAsync(currentIntegration.UserId);
                            
                            // Re-fetch the integration from the DB to get the brand new AccessToken!
                            currentIntegration = (await emailIntegrationRepository.FindIntegrationAsync(currentIntegration.UserId))!;
                        }

                        // 6. CONNECT TO IMAP 
                        using (var client = new ImapClient())
                        {
                            client.Timeout = 20000; 
                            await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);

                            // 7. OAUTH2 AUTHENTICATION (NO PASSWORDS NEEDED)
                            // We pass the email and the valid Access Token.
                            var oauth2 = new SaslMechanismOAuth2(currentIntegration.ConnectedEmail, currentIntegration.AccessToken);
                            await client.AuthenticateAsync(oauth2, stoppingToken);

                            // 8. YOUR OLD CODE: PROCESS EMAILS FOR THIS INBOX
                            var inbox = client.Inbox;
                            await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                            var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);
                            Console.WriteLine($"[IMAP] Found {uids.Count} unread emails.");

                            foreach (var uid in uids)
                            {
                                var message = await inbox.GetMessageAsync(uid, stoppingToken);
                                var subject = message.Subject?.Trim() ?? "";
                                
                                Console.WriteLine($"[PROCESS] Examining email: \"{subject}\"");

                                var vacancy = await vacancyRepository.GetByTitleAsync(subject);
                                if (vacancy == null)
                                {
                                    Console.WriteLine($"[SKIP] No vacancy found matching title: \"{subject}\"");
                                    continue; 
                                }

                                var fullDossier = new StringBuilder();
                                fullDossier.AppendLine("=== EMAIL BODY ===");
                                fullDossier.AppendLine(message.TextBody ?? string.Empty);

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
                                            
                                            var text = await parsingService.ExtractTextAsync(stream);
                                            fullDossier.AppendLine($"--- FILE: {part.FileName} ---");
                                            fullDossier.AppendLine(text);
                                            
                                            stream.Position = 0;
                                            resumeUrl = await _storageService.UploadAsync(stream, part.FileName, "application/pdf");
                                            hasPdf = true;
                                        }
                                    }
                                }

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
                                    Console.WriteLine($"[SUCCESS] Candidate saved: {request.Email}");
                                }

                                // Mark email as read so we don't process it again
                                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                            }

                            // Disconnect from THIS user's inbox before moving to the next user in the foreach loop
                            await client.DisconnectAsync(true, stoppingToken);
                        }
                    } // End of foreach (integrations)
                } // End of using (scope) - Database connection is closed safely
            }
            catch (Exception ex)
            {
                var errorMsg = $"[FATAL ERROR] {DateTime.Now:HH:mm:ss}: {ex.Message}";
                Console.WriteLine(errorMsg);
                _logger.LogError(ex, "Background worker error occurred.");
            }

            // Sleep for 30 seconds before starting the whole process again
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}