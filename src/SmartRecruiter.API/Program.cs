using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartRecruiter.API.Middleware;
using SmartRecruiter.API.Workers;
using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Application.Validators;
using SmartRecruiter.Domain.Interfaces;
using SmartRecruiter.Infrastructure.Persistance;
using SmartRecruiter.Infrastructure.Repositories;
using SmartRecruiter.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your Vite frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IStorageService, BlobStorageService>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobVacancyRepository, JobVacancyRepository>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddScoped<IAuthService, IdentityAuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFileParsingService, PdfParsingService>();
builder.Services.AddHttpClient<IAiService, OpenAiService>();
builder.Services.AddScoped<JobVacancyService>();
builder.Services.AddScoped<CandidateService>();
builder.Services.AddHostedService<EmailBackgroundWorker>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCandidateValidator>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();