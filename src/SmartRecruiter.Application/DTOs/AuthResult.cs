namespace SmartRecruiter.Application.DTOs;

public record AuthResult(bool Succeeded, List<string> Errors );