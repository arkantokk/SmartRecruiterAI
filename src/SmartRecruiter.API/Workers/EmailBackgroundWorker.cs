using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.API.Workers;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundWorker> _logger;

    public EmailBackgroundWorker(IServiceScopeFactory scopeFactory, ILogger<EmailBackgroundWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📧 Email Worker Started running...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Створюємо "Scope" (окрему зону пам'яті для однієї ітерації)
                using (var scope = _scopeFactory.CreateScope())
                {
                    var candidateService = scope.ServiceProvider.GetRequiredService<CandidateService>();
                    var parsingService = scope.ServiceProvider.GetRequiredService<IFileParsingService>();
                    var vacancyRepository = scope.ServiceProvider.GetRequiredService<IJobVacancyRepository>(); 
                    
                    _logger.LogInformation("🔍 Checking for new emails...");

                    // --- СИМУЛЯЦІЯ ---
                    // 1. Отримуємо вакансію (нам треба реальний ID з бази, інакше впаде)
                    // Тобі треба вставити сюди ID тієї вакансії, яку ти створив через Swagger
                    var vacancyId = Guid.Parse("4fb1148a-e523-4e6a-b118-b447da700392"); 
                    
                    // Перевірка, чи вакансія існує (щоб не крашнулось)
                    var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);
                    if (vacancy != null)
                    {
                        _logger.LogInformation($"📨 Processing email for vacancy: {vacancy.Title}");

                        // 2. Симулюємо файл PDF (ніби прийшов поштою)
                        var fakePdfContent = "Це тестове резюме. Я експерт з C# і .NET Core. Хочу працювати у вас.";
                        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fakePdfContent));

                        // 3. Читаємо файл (твоїм новим сервісом)
                        var extractedText = await parsingService.ExtractTextAsync(stream);
                        
                        // 4. Створюємо запит
                        var request = new CreateCandidateRequest
                        {
                            FirstName = "Auto",
                            LastName = "WorkerBot",
                            Email = "bot@test.com",
                            JobVacancyId = vacancyId
                        };

                        // 5. Зберігаємо (тут всередині спрацює AI)
                        var newId = await candidateService.RegisterCandidateAsync(request);
                        
                        _logger.LogInformation($"✅ Candidate created! ID: {newId}");
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Vacancy not found! Check GUID.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error in Email Worker");
            }

            // Чекаємо 1 хвилину (щоб ти встиг побачити логи і пам'ять не стрибала)
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}