using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;

namespace SmartRecruiter.Application.Services;

public class GmailAuthService : IGmailAuthService
{
    private readonly IOAuthClient _authClient;
    private readonly IEmailIntegrationRepository _emailIntegrationRepository;

    public GmailAuthService(IOAuthClient authClient, IEmailIntegrationRepository emailIntegrationRepository)
    {
        _authClient = authClient;
        _emailIntegrationRepository = emailIntegrationRepository;
    }

    public string GetAuthorizationUrl(string userId)
    {
        var url = _authClient.GetAuthorizationUrl(userId);
        return url;
    }

    public async Task HandleCallbackAsync(string code, string userId)
    {
        var response = await _authClient.ExchangeCodeAsync(code);

        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresInSeconds);

        var integration = await _emailIntegrationRepository.FindIntegrationAsync(userId);

        if (integration == null)
        {
            var newIntegration = new EmailIntegration(
                userId,
                "Google",
                response.Email,
                response.AccessToken,
                response.RefreshToken,
                expiresAt
            );

            await _emailIntegrationRepository.AddIntegrationAsync(newIntegration);
        }
        else
        {
            integration.ChangeAccessToken(response.AccessToken, expiresAt);
            await _emailIntegrationRepository.UpdateIntegrationAsync(integration);
        }
    }

    public async Task RefreshIntegrationAsync(string userId)
    {
        var integration = await _emailIntegrationRepository.FindIntegrationAsync(userId);
        if (integration == null)
        {
            throw new Exception("integration not found");
        }

        var response = await _authClient.RefreshTokenAsync(integration.RefreshToken);
        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresInSeconds);
        integration.ChangeAccessToken(response.AccessToken, expiresAt, response.RefreshToken);
        await _emailIntegrationRepository.UpdateIntegrationAsync(integration);
    }

    public async Task<IntegrationStatusDto> IntegrationStatus(string userId)
    {
        var integration = await _emailIntegrationRepository.FindIntegrationAsync(userId);

        if (integration == null)
        {
            return new IntegrationStatusDto(false, null);
        }

        var isValid = await _authClient.VerifyAccessTokenAsync(integration.AccessToken);

        if (!isValid)
        {
            await RefreshIntegrationAsync(userId);
        }

        return new IntegrationStatusDto(true, integration.ConnectedEmail);
    }
}