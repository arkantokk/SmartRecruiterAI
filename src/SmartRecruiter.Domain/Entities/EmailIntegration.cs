namespace SmartRecruiter.Domain.Entities;

public class EmailIntegration
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Provider { get; private set; } 
    public string ConnectedEmail { get; private set; } 
    public string AccessToken { get; private set; } 
    public string RefreshToken { get; private set; }
    public DateTimeOffset AccessTokenExpiresAt { get; private set; }

    public EmailIntegration(string userId, string provider, string connectedEmail, string accessToken, string refreshToken, DateTimeOffset accessTokenExpiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Provider = provider;
        ConnectedEmail = connectedEmail;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiresAt = accessTokenExpiresAt;
    }

    public void ChangeAccessToken(string newToken, DateTimeOffset newDate, string? refreshToken = null)
    {
        AccessToken = newToken;
        AccessTokenExpiresAt = newDate;
        if (refreshToken != null)
        {
            RefreshToken = refreshToken;
        }
    }
    
}