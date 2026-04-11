using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface ICandidateQueries
{
    Task<PagedResponse<CandidateDto>> GetCandidatesForVacancyIdAsync(
        Guid vacancyId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = null,
        string? statusTab = "Active",
        string? archiveFilter = "All");

    Task<PagedResponse<CandidateDto>> GetCandidatesForUserAsync(string userId, int pageNumber, int pageSize,
        string? searchTerm = null,
        string? sortBy = null);
}