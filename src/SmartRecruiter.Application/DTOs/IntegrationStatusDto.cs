namespace SmartRecruiter.Application.DTOs;

public record IntegrationStatusDto(
    bool IsConnected,
    string? Email
    );