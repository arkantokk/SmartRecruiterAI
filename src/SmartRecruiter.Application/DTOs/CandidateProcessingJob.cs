namespace SmartRecruiter.Application.DTOs;

public record CandidateProcessingJob(
    string FirstName,
    string LastName,
    string Email,
    Guid JobVacancyId,
    string ResumeText,
    string? ResumeUrl
);