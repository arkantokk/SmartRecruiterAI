using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Interfaces;

public interface IGmailAuthService
{
    string GetAuthorizationUrl(string userId);
    Task HandleCallbackAsync(string code, string userId);
    Task RefreshIntegrationAsync(string userId);
    Task<IntegrationStatusDto> IntegrationStatus(string userId);
}