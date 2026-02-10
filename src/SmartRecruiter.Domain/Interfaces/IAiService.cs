using SmartRecruiter.Domain.ValueObjects;

namespace SmartRecruiter.Domain.Interfaces;

public interface IAiService
{
    Task<CandidateEvaluation> EvaluateCandidateAsync(string resumeText, string aiPromptTemplate);
}