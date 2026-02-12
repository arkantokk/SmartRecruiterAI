using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Enums;
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

    public async Task<IEnumerable<Candidate>> GetAllCandidatesAsync()
    {
        return await _context.Candidates.ToListAsync();
    }

    public async Task UpdateStatusAsync(Guid id, CandidateStatus newStatus)
    {
        var candidate = await _context.Candidates.FindAsync(id);
        if (candidate == null) throw new KeyNotFoundException($"Candidate {id} not found");

        candidate.ChangeStatus(newStatus);
        await _context.SaveChangesAsync();
    }
}