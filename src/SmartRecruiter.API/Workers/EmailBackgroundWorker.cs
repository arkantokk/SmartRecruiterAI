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
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Worker initialized.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var emailRepo = scope.ServiceProvider.GetRequiredService<IEmailIntegrationRepository>();
                    var authService = scope.ServiceProvider.GetRequiredService<IGmailAuthService>();
                    var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                    var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                    var vacancyRepo = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>();

                    var integrations = await emailRepo.GetAllIntegrationAsync();

                    foreach (var integration in integrations)
                    {
                        var current = integration;
                        
                        try 
                        {
                            Console.WriteLine($"\n[ACCOUNT] Processing: {current.ConnectedEmail}");

                            if (current.AccessTokenExpiresAt < DateTimeOffset.UtcNow.AddMinutes(2))
                            {
                                Console.WriteLine($"[AUTH] Refreshing: {current.ConnectedEmail}");
                                await authService.RefreshIntegrationAsync(current.UserId);
                                current = (await emailRepo.FindIntegrationAsync(current.UserId))!;
                            }

                            using (var client = new ImapClient())
                            {
                                await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);
                                var oauth2 = new SaslMechanismOAuth2(current.ConnectedEmail, current.AccessToken);
                                
                                try 
                                {
                                    await client.AuthenticateAsync(oauth2, stoppingToken);
                                }
                                catch (AuthenticationException)
                                {
                                    Console.WriteLine($"[AUTH] Forced refresh for {current.ConnectedEmail}...");
                                    await authService.RefreshIntegrationAsync(current.UserId);
                                    current = (await emailRepo.FindIntegrationAsync(current.UserId))!;
                                    oauth2 = new SaslMechanismOAuth2(current.ConnectedEmail, current.AccessToken);
                                    await client.AuthenticateAsync(oauth2, stoppingToken);
                                }

                                var inbox = client.Inbox;
                                await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);
                                var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);
                                
                                if (uids.Count > 0)
                                {
                                    Console.WriteLine($"[IMAP] {current.ConnectedEmail}: {uids.Count} new emails.");
                                    var summaries = await inbox.FetchAsync(uids, MessageSummaryItems.Envelope | MessageSummaryItems.UniqueId, stoppingToken);
                                    var userVacancies = await vacancyRepo.GetUserVacancies(current.UserId);

                                    foreach (var summary in summaries)
                                    {
                                        var uid = summary.UniqueId;
                                        var subject = summary.Envelope.Subject?.Trim() ?? "";
                                        await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                                        
                                        var vacancy = userVacancies.FirstOrDefault(v => 
                                            subject.ToLower().Contains(v.Title.ToLower().Trim()));

                                        if (vacancy == null) continue;

                                        var message = await inbox.GetMessageAsync(uid, stoppingToken);
                                        var fullDossier = new StringBuilder();
                                        fullDossier.AppendLine(message.TextBody ?? string.Empty);
                                        bool hasPdf = false; string? resumeUrl = null;

                                        foreach (var attachment in message.Attachments)
                                        {
                                            if (attachment is MimePart part && part.FileName.ToLower().EndsWith(".pdf"))
                                            {
                                                using (var stream = new MemoryStream())
                                                {
                                                    await part.Content.DecodeToAsync(stream, stoppingToken);
                                                    stream.Position = 0;
                                                    var text = await parsingService.ExtractTextAsync(stream);
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
                                            Console.WriteLine($"[SUCCESS] Registered: {request.Email}");
                                        }
                                    }
                                }
                                await client.DisconnectAsync(true, stoppingToken);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Skipping {current.ConnectedEmail} due to: {ex.Message}");
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global worker loop error.");
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}