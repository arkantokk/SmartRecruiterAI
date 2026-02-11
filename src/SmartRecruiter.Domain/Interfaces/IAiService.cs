using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.ValueObjects;

namespace SmartRecruiter.Domain.Interfaces;

public interface IAiService
{
    Task<CandidateEvaluation> EvaluateCandidateAsync(Candidate candidate, JobVacancy jobVacancy);
}