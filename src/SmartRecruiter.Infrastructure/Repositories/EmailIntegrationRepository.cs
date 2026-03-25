using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Infrastructure.Persistance;

namespace SmartRecruiter.Infrastructure.Repositories;

public class EmailIntegrationRepository : IEmailIntegrationRepository
{
    private readonly ApplicationDbContext _context;

    public EmailIntegrationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmailIntegration?> FindIntegrationAsync(string userId)
    {
        return await _context.EmailIntegrations
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task AddIntegrationAsync(EmailIntegration emailIntegration)
    {
        await _context.EmailIntegrations.AddAsync(emailIntegration);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateIntegrationAsync(EmailIntegration emailIntegration)
    {
        _context.EmailIntegrations.Update(emailIntegration);
        await _context.SaveChangesAsync();
    }

    public async Task<List<EmailIntegration>> GetAllIntegrationAsync()
    {
        return await _context.EmailIntegrations.ToListAsync();
    }
}