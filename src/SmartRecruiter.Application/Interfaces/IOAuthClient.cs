using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface IOAuthClient
{
    string GetAuthorizationUrl(string state);
    Task<OAuthTokenResponse> ExchangeCodeAsync(string code);
    Task<TokenRefreshResponse> RefreshTokenAsync(string refreshToken);
    // check integration
}