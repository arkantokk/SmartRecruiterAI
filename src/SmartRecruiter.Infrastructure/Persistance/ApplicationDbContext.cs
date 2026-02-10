using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Infrastructure.Persistance;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<JobVacancy> JobVacancies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidate>()
            .OwnsOne(c => c.Evaluation);

        base.OnModelCreating(modelBuilder);
    }
}