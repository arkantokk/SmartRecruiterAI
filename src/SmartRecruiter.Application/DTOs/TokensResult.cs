namespace SmartRecruiter.Application.DTOs;

public record TokensResult(bool IsAuth, string Token, string refreshToken);