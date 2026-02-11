using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Application.Services;

public class JobVacancyService
{
    private readonly IJobVacancyRepository _jobVacancyRepository;

    public JobVacancyService(IJobVacancyRepository jobVacancyRepository)
    {
        _jobVacancyRepository = jobVacancyRepository;
    }

    public async Task<Guid> AddJobVacancyAsync(CreateJobVacancyRequest jobVacancyRequest)
    {
        var job = new JobVacancy(jobVacancyRequest.Title, jobVacancyRequest.AiPromptTemplate);
        await _jobVacancyRepository.AddAsync(job);

        return job.Id;
    }
}