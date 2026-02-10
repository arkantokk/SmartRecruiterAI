using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Infrastructure.Persistance;

namespace SmartRecruiter.Infrastructure.Repositories;

public class CandidateRepository : ICandidateRepository
{
    private readonly ApplicationDbContext _context;

    public CandidateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Candidate candidate)
    {
        await _context.Candidates.AddAsync(candidate);
        await _context.SaveChangesAsync();
    }

    public async Task<Candidate?> GetByIdAsync(Guid id)
    {
        return await _context.Candidates.FindAsync(id);
    }
}