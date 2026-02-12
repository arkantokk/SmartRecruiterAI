public record CandidateDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? ResumeUrl,
    int Score,
    string Summary,
    List<string> Skills
);