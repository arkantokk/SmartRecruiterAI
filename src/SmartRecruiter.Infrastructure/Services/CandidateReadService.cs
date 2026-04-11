using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Infrastructure.Persistance;

namespace SmartRecruiter.Infrastructure.Services;

public class CandidateReadService : ICandidateQueries
{
    private readonly ApplicationDbContext _context;
    private readonly IStorageService _storageService;

    public CandidateReadService(ApplicationDbContext context, IStorageService storageService)
    {
        _context = context;
        _storageService = storageService;
    }

    public async Task<PagedResponse<CandidateDto>> GetCandidatesForVacancyIdAsync(Guid vacancyId, int pageNumber, int pageSize)
    {
        var baseQuery = _context.Candidates
            .AsNoTracking()
            .Where(c => c.JobVacancyId == vacancyId );
        var totalCount = await baseQuery.CountAsync();
        var candidates = await baseQuery
            .OrderByDescending(c => c.Evaluation.Score)
            .Skip((pageNumber-1)*pageSize)
            .Take((pageSize))
            .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.ResumeUrl,
                    Score = c.Evaluation != null ? c.Evaluation.Score : 0,
                    Summary = c.Evaluation != null ? c.Evaluation.Summary : string.Empty,
                    Skills = c.Evaluation != null ? c.Evaluation.Skills : new List<string>(),
                    Status = c.Status.ToString()
                }
            )
            .ToListAsync();
        var items = candidates
            .Select(c => new CandidateDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.ResumeUrl != null ? _storageService.GenerateReadOnlyUrl(c.ResumeUrl, TimeSpan.FromHours(1)) : null,
                c.Score,
                c.Summary,
                c.Skills,
                c.Status
                )).ToList();
        return new PagedResponse<CandidateDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<PagedResponse<CandidateDto>> GetCandidatesForUserAsync(string userId, int pageNumber, int pageSize)
    {
        var baseQuery = _context.Candidates
            .AsNoTracking()
            .Where(c => _context.JobVacancies.Any(v => v.Id == c.JobVacancyId && v.UserId == userId));
        var totalCount = await baseQuery.CountAsync();
        var candidates = await baseQuery
            .OrderByDescending(c => c.Evaluation.Score)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
                {
                    c.Id,
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.ResumeUrl,
                    Score = c.Evaluation != null ? c.Evaluation.Score : 0,
                    Summary = c.Evaluation != null ? c.Evaluation.Summary : string.Empty,
                    Skills = c.Evaluation != null ? c.Evaluation.Skills : new List<string>(),
                    Status = c.Status.ToString()
                }
            )
            .ToListAsync();
        var items = candidates
            .Select(c => new CandidateDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.ResumeUrl != null ? _storageService.GenerateReadOnlyUrl(c.ResumeUrl, TimeSpan.FromHours(1)) : null,
                c.Score,
                c.Summary,
                c.Skills,
                c.Status
            )).ToList();
        return new PagedResponse<CandidateDto>(items, totalCount, pageNumber, pageSize);

    }
}