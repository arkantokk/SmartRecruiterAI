using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface ICandidateRepository
{
    Task AddAsync(Candidate candidate);
    Task<Candidate?> GetByIdAsync(Guid id);
}