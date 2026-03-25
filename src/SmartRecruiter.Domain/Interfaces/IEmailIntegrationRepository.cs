using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Domain.Interfaces;

public interface IEmailIntegrationRepository
{
    Task AddIntegrationAsync(EmailIntegration emailIntegration);
    Task<EmailIntegration?> FindIntegrationAsync(string userId);
    Task UpdateIntegrationAsync(EmailIntegration emailIntegration);
}