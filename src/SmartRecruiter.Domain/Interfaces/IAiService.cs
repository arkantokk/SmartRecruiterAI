using SmartRecruiter.Domain.DTOs;
using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface IAiService
{
    Task<AiAnalysisResult> EvaluateCandidateAsync(JobVacancy vacancy, string resumeText);
}