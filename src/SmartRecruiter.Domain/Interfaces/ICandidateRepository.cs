using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Enums;

namespace SmartRecruiter.Domain.Interfaces;

public interface ICandidateRepository
{
    Task AddAsync(Candidate candidate);
    Task<Candidate?> GetByIdAsync(Guid id);
    Task<IEnumerable<Candidate>> GetAllCandidatesAsync();
    Task UpdateStatusAsync(Guid id, CandidateStatus newStatus);
}