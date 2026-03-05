using SmartRecruiter.Domain.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(TokenUserDto user);
}