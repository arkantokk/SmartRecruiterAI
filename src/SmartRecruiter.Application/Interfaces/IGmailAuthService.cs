namespace SmartRecruiter.Application.Interfaces;

public interface IGmailAuthService
{
    string GetAuthorizationUrl(string userId);
    Task HandleCallbackAsync(string code, string userId);
}