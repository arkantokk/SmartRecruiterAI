using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface ICandidateQueries
{
    Task<PagedResponse<CandidateDto>> GetCandidatesForVacancyIdAsync(Guid vacancyId, int pageNumber, int pageSize);
}