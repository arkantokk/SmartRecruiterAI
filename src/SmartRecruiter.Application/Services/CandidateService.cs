using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Application.Services;

public class CandidateService
{
    private readonly ICandidateRepository _repository;

    public CandidateService(ICandidateRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Guid> RegisterCandidateAsync(CreateCandidateRequest request)
    {
        var candidate = new Candidate(request.FirstName, request.LastName, request.Email, request.JobVacancyId);
        await _repository.AddAsync(candidate);
        return candidate.Id;
    }
}