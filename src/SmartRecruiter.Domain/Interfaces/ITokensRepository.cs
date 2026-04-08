using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface ITokensRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken refreshToken);
}