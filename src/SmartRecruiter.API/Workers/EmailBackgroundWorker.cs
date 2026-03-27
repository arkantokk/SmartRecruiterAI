using System.Text;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
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
    
    private const string MailServer = "imap.gmail.com";
    private const int MailPort = 993;

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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var emailIntegrationRepository = scope.ServiceProvider.GetRequiredService<IEmailIntegrationRepository>();
                    var gmailAuthService = scope.ServiceProvider.GetRequiredService<IGmailAuthService>();
                    var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                    var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                    var vacancyRepository = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>();

                    var integrations = await emailIntegrationRepository.GetAllIntegrationAsync();

                    foreach (var integration in integrations)
                    {
                        var currentIntegration = integration; 

                        if (currentIntegration.AccessTokenExpiresAt < DateTimeOffset.UtcNow.AddMinutes(2))
                        {
                            await gmailAuthService.RefreshIntegrationAsync(currentIntegration.UserId);
                            currentIntegration = (await emailIntegrationRepository.FindIntegrationAsync(currentIntegration.UserId))!;
                        }

                        using (var client = new ImapClient())
                        {
                            client.Timeout = 20000; 
                            await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);

                            var oauth2 = new SaslMechanismOAuth2(currentIntegration.ConnectedEmail, currentIntegration.AccessToken);
                            await client.AuthenticateAsync(oauth2, stoppingToken);

                            var inbox = client.Inbox;
                            await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                            var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);

                            foreach (var uid in uids)
                            {
                                var message = await inbox.GetMessageAsync(uid, stoppingToken);
                                
                                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                                
                                var subject = message.Subject?.Trim() ?? "";
                                
                                var vacancy = await vacancyRepository.GetByTitleAndUserIdAsync(subject, currentIntegration.UserId);
                                if (vacancy == null)
                                {
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
                                }
                            }

                            await client.DisconnectAsync(true, stoppingToken);
                        }
                    } 
                } 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background worker error occurred.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}