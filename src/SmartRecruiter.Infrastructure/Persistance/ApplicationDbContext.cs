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
        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.OwnsOne(c => c.Evaluation, evaluation =>
            {
                evaluation.Property(e => e.Pros)
                    .HasConversion(
                        v => string.Join(';', v),                // List -> String
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()); // String -> List

                evaluation.Property(e => e.Cons)
                    .HasConversion(
                        v => string.Join(';', v),
                        v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList());
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}