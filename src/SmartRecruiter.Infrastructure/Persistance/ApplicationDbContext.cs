using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartRecruiter.Domain.Entities;

namespace SmartRecruiter.Infrastructure.Persistance;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<JobVacancy> JobVacancies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // moved to top to avoid inheritance errors
        modelBuilder.Entity<Candidate>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.Evaluation, a =>
            {
                a.Property(e => e.Pros)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
                
                a.Property(e => e.Cons)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
                
                a.Property(e => e.Skills)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null), // C# -> JSON String
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)); // JSON String -> C#
            });
        });
    }
}