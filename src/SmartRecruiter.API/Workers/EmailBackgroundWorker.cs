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
        var mailUser = _configuration["EmailSettings:Email"];
        var mailPassword = _configuration["EmailSettings:Password"];

        if (string.IsNullOrEmpty(mailUser) || string.IsNullOrEmpty(mailPassword))
        {
            _logger.LogError("Email credentials missing");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.Timeout = 10000;
                    await client.ConnectAsync(MailServer, MailPort, true, stoppingToken);
                    await client.AuthenticateAsync(mailUser, mailPassword, stoppingToken);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite, stoppingToken);

                    var uids = await inbox.SearchAsync(SearchQuery.NotSeen, stoppingToken);

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
                                var subject = message.Subject.Trim();

                                var vacancy = await vacancyRepository.GetByTitleAsync(subject);

                                if (vacancy != null)
                                {
                                    var fullDossier = new StringBuilder();

                                    // Collect Email Body
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
                                                var text = await parsingService.ExtractTextAsync(stream);
                                                fullDossier.AppendLine($"--- FILE: {part.FileName} ---");
                                                fullDossier.AppendLine(text);
                                                stream.Position = 0;
                                                resumeUrl = await _storageService.UploadAsync(stream, part.FileName,
                                                    "application/pdf");

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

                                await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true, stoppingToken);
                            }
                        }
                    }

                    await client.DisconnectAsync(true, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Worker Error: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}