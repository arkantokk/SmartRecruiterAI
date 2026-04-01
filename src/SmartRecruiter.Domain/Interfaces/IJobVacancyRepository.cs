using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface IJobVacancyRepository
{
    Task AddAsync(JobVacancy jobVacancy);
    Task<JobVacancy?> GetByIdAsync(Guid id);
    Task<JobVacancy?> GetByTitleAsync(string title); // needs to be removed or used differently because of risk when checking two emails with same job title
    Task<JobVacancy?> GetByTitleAndUserIdAsync(string title, string userId);
    Task<IEnumerable<JobVacancy>> GetUserVacancies(string userId);
    Task UpdateJobVacancyAsync(JobVacancy jobVacancy);
}