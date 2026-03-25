namespace SmartRecruiter.Application.DTOs;

public record TokenRefreshResponse(
    string AccessToken,
    string? RefreshToken,
    int ExpiresInSeconds
    );