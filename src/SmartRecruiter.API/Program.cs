using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartRecruiter.API.Workers;
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
builder.Services.AddSingleton<IStorageService, BlobStorageService>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobVacancyRepository, JobVacancyRepository>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();