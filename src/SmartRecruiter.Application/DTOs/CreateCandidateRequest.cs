namespace SmartRecruiter.Application.DTOs;

public class CreateCandidateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid JobVacancyId { get; set; } = Guid.Empty;
}