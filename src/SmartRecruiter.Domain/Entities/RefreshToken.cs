namespace SmartRecruiter.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public string UserId { get; private set; }

    public RefreshToken(string token, DateTime expiryDate, string userId)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiryDate = expiryDate;
        IsRevoked = false;
        UserId = userId;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
    
    
}