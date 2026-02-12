using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Infrastructure.Persistance;

namespace SmartRecruiter.Infrastructure.Repositories;

public class JobVacancyRepository : IJobVacancyRepository
{
    private readonly ApplicationDbContext _context;

    public JobVacancyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JobVacancy jobVacancy)
    {
       await _context.JobVacancies.AddAsync(jobVacancy);
       await _context.SaveChangesAsync();
    }

    public async Task<JobVacancy?> GetByIdAsync(Guid id)
    {
        return await _context.JobVacancies.FindAsync(id);
    }

    public async Task<JobVacancy?> GetByTitleAsync(string title)
    {
        return await _context.JobVacancies.FirstOrDefaultAsync(v => v.Title.ToLower() == title.ToLower());
    }
}