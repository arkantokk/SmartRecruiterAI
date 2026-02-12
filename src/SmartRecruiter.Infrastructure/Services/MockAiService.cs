/*using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Domain.ValueObjects;

namespace SmartRecruiter.Infrastructure.Services;

public class MockAiService : IAiService
{
    private readonly Random _random = new Random();

    public async Task<CandidateEvaluation> EvaluateCandidateAsync(Candidate candidate, JobVacancy vacancy, string resumeText)
    {
        await Task.Delay(2000); // Чекаємо 2 секунди

        // 2. Генерація фейкових даних
        var score = _random.Next(50, 100); // Оцінка від 50 до 99
        
        var pros = new List<string> 
        { 
            "Має профільну освіту", 
            "Добре структуроване резюме" 
        };
        var skills = new List<string>();
        var firstName = "Joe";
        var lastName = "Doe";
        var cons = new List<string> 
        { 
            "Малий досвід роботи з Docker", 
            "Не вказав рівень англійської" 
        };

        var summary = $"ШІ-аналіз для кандидата {candidate.FirstName} {candidate.LastName}.  на вакансію {vacancy.Title}" +
                      $"Попередній висновок: кандидат виглядає перспективно, але потребує технічної співбесіди.";

        // 3. Повертаємо Value Object
        return new CandidateEvaluation(score, summary, pros, cons, skills);
    }
}*/