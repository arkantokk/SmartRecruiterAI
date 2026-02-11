using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface IJobVacancyRepository
{
    Task AddAsync(JobVacancy jobVacancy);
    Task<JobVacancy?> GetByIdAsync(Guid id);
}