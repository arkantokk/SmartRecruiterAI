using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<bool> RevokeAsync(string refreshToken);
    Task<TokensResult> RefreshAsync(string refreshToken);
    Task<AuthResult> GoogleLoginAsync(string googleToken);
}