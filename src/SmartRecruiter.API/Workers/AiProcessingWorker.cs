using SmartRecruiter.Application.Services;

namespace SmartRecruiter.API.Workers;

public class AiProcessingWorker : BackgroundService
{
    private readonly CandidateQueueService _queueService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiProcessingWorker> _logger;

    public AiProcessingWorker(
        CandidateQueueService queueService,
        IServiceScopeFactory scopeFactory,
        ILogger<AiProcessingWorker> logger)
    {
        _queueService = queueService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] AI Processing Worker started.");

        await foreach (var request in _queueService.ReadAllAsync(stoppingToken))
        {
            try
            {
                Console.WriteLine($"[AI WORKER] Started analyzing resume for: {request.Email}");

                using var scope = _scopeFactory.CreateScope(); // creating scope for db call
                var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>(); // getting service to interact with db

                var candidateId = await candidateService.RegisterCandidateAsync(request);

                Console.WriteLine($"[AI WORKER] SUCCESS! Saved candidate {candidateId} for {request.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[AI WORKER] Error processing candidate {request.Email}");
            }
        }
    }
}