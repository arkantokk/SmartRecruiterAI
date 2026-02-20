using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
}