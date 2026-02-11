using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Application.Services;

public class CandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly IJobVacancyRepository _vacancyRepository;
    private readonly IAiService _aiService;
    public CandidateService(ICandidateRepository repository, IAiService aiService, IJobVacancyRepository jobVacancyRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _aiService = aiService;
        _vacancyRepository = jobVacancyRepository;
    }

    public async Task<Guid> RegisterCandidateAsync(CreateCandidateRequest request)
    {
        var candidate = new Candidate(request.FirstName, request.LastName, request.Email, request.JobVacancyId);
        var jobVacancy = await _vacancyRepository.GetByIdAsync(request.JobVacancyId);
        if (jobVacancy == null) throw new ArgumentNullException();
        var evaluation = await _aiService.EvaluateCandidateAsync(candidate, jobVacancy);
        candidate.Evaluate(evaluation.Score, evaluation.Summary, evaluation.Pros, evaluation.Cons);
        await _repository.AddAsync(candidate);
        return candidate.Id;
    }

    public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
    {
        return await _repository.GetAllCandidatesAsync();
    }
}