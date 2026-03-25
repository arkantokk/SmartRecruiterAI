namespace SmartRecruiter.Application.DTOs;

public record OAuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresInSeconds,
    string Email
    );