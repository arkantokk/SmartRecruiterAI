using SmartRecruiter.Application.Interfaces;
using SmartRecruiter.Domain.Interfaces;
using Moq;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Services;
using SmartRecruiter.Domain.DTOs;
using SmartRecruiter.Domain.Entities;
using SmartRecruiter.Domain.Enums;
using Xunit;

namespace SmartRecruiter.Application.Tests.Services;

public class CandidateServiceTests
{
    private readonly Mock<ICandidateRepository> _repository;
    private readonly Mock<IJobVacancyRepository> _vacancyRepository;
    private readonly Mock<IAiService> _aiService;
    private readonly Mock<IStorageService> _storageService;
    private readonly Mock<ICandidateQueries> _candidateQueries;

    private readonly CandidateService _service;


    public CandidateServiceTests()
    {
        _repository = new Mock<ICandidateRepository>();
        _vacancyRepository = new Mock<IJobVacancyRepository>();
        _aiService = new Mock<IAiService>();
        _storageService = new Mock<IStorageService>();
        _candidateQueries = new Mock<ICandidateQueries>();

        _service = new CandidateService(
            _repository.Object,
            _aiService.Object,
            _vacancyRepository.Object,
            _storageService.Object,
            _candidateQueries.Object
        );
    }

    [Fact]
    public async Task RegisterCandidateAsync_Should_ThrowException_When_VacancyNotFound()
    {
        var request = new CreateCandidateRequest { JobVacancyId = Guid.NewGuid() };

        _vacancyRepository.Setup(repo => repo.GetByIdAsync(request.JobVacancyId))
            .ReturnsAsync((JobVacancy)null);

        var exception = await Assert
            .ThrowsAsync<KeyNotFoundException>(() => _service.RegisterCandidateAsync(request));
        Assert.Equal($"Vacancy {request.JobVacancyId} not found", exception.Message);
    }

    [Fact]
    public async Task RegisterCandidateAsync_Should_SaveCandidate_When_DataIsValid()
    {
        var request = new CreateCandidateRequest
        {
            JobVacancyId = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            ResumeText = "I am a fantastic C# developer."
        };

        var fakeVacancy = new JobVacancy("Fake Title", "Fake Description", "user123");

        typeof(JobVacancy).GetProperty("Id")?.SetValue(fakeVacancy, request.JobVacancyId);

        _vacancyRepository.Setup(repo => repo.GetByIdAsync(request.JobVacancyId))
            .ReturnsAsync(fakeVacancy);

        var fakeAiResult = new AiAnalysisResult
            (
                "Jane",
                "Doe",
                95,
                "Excellent match.",
                new List<string> { "C#", "React" },
                new List<string> { "C#", "React" },
                new List<string> { "C#", "React" }
                );

        _aiService.Setup(x => x.EvaluateCandidateAsync(fakeVacancy, request.ResumeText))
            .ReturnsAsync(fakeAiResult);

        await _service.RegisterCandidateAsync(request);
        _repository.Verify(repo => repo.AddAsync(It.IsAny<Candidate>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_Should_Update_When_DataIsValid()
    {
        var requestId = Guid.NewGuid();
        var requestEnum = CandidateStatus.Applied;
        await _service.UpdateStatusAsync(requestId, requestEnum);
        _repository.Verify(repo => repo.UpdateStatusAsync(requestId, requestEnum));
    }
}