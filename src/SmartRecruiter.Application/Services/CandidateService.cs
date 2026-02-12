using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Application.Services;

public class CandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly IJobVacancyRepository _vacancyRepository;
    private readonly IAiService _aiService;
    private readonly IStorageService _storageService;

    public CandidateService(
        ICandidateRepository repository,
        IAiService aiService,
        IJobVacancyRepository jobVacancyRepository,
        IStorageService storageService
        )
    {
        _repository = repository;
        _aiService = aiService;
        _vacancyRepository = jobVacancyRepository;
        _storageService = storageService;
    }

    public async Task<Guid> RegisterCandidateAsync(CreateCandidateRequest request)
    {
        var jobVacancy = await _vacancyRepository.GetByIdAsync(request.JobVacancyId);
        if (jobVacancy == null)
            throw new KeyNotFoundException($"Vacancy {request.JobVacancyId} not found");

        var aiResult = await _aiService.EvaluateCandidateAsync(jobVacancy, request.ResumeText);

        var firstName = !string.IsNullOrWhiteSpace(aiResult.FirstName)
            ? aiResult.FirstName
            : request.FirstName;

        var lastName = !string.IsNullOrWhiteSpace(aiResult.LastName)
            ? aiResult.LastName
            : request.LastName;

        var candidate = new Candidate(
            firstName,
            lastName,
            request.Email,
            request.JobVacancyId);
        
        candidate.Evaluate(
            aiResult.Score,
            aiResult.Summary,
            aiResult.Pros,
            aiResult.Cons,
            aiResult.Skills);
        if (!string.IsNullOrWhiteSpace(request.ResumeUrl))
        {
            candidate.SetResume(request.ResumeUrl);
        }
        await _repository.AddAsync(candidate);

        return candidate.Id;
    }

    public async Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync()
    {
        var candidates = await _repository.GetAllCandidatesAsync();

        return candidates.Select(c => new CandidateDto(
            c.Id,
            c.FirstName,
            c.LastName,
            c.Email,
            c.ResumeUrl != null ? _storageService.GenerateReadOnlyUrl(c.ResumeUrl, TimeSpan.FromHours(1)) : null,
            c.Evaluation.Score,
            c.Evaluation.Summary,
            c.Evaluation.Skills
        ));
    }
}