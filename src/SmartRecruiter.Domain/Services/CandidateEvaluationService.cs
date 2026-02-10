using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Domain.Services;

public class CandidateEvaluationService
{
    private readonly IAiService _aiService;

    public CandidateEvaluationService(IAiService aiService)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
    }

    public async Task EvaluateAsync(Candidate candidate, JobVacancy vacancy)
    {
        if (candidate == null) throw new ArgumentNullException(nameof(candidate));
        if (vacancy == null) throw new ArgumentNullException(nameof(vacancy));
        
        if (candidate.JobVacancyId != vacancy.Id)
        {
            throw new InvalidOperationException("Candidate applies to a different vacancy.");
        }
        string resumeText = "Temporary text from URL: " + candidate.ResumeUrl;
        var evaluation = await _aiService.EvaluateCandidateAsync(resumeText, vacancy.AiPromptTemplate);
        candidate.UpdateAssessment(evaluation);
    }
}