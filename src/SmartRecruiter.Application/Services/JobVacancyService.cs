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

    public async Task<Guid> AddJobVacancyAsync(CreateJobVacancyRequest jobVacancyRequest, string userId)
    {
        var job = new JobVacancy(jobVacancyRequest.Title, jobVacancyRequest.AiPromptTemplate, userId);
        await _jobVacancyRepository.AddAsync(job);

        return job.Id;
    }

    public async Task<IEnumerable<JobVacancy>> GetUserVacanciesAsync(string userId)
    {
        var vacancies = await _jobVacancyRepository.GetUserVacancies(userId);
        return vacancies;
    }
}