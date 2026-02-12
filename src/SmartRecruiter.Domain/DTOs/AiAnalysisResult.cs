namespace SmartRecruiter.Domain.DTOs;

public record AiAnalysisResult(
    string FirstName,
    string LastName,
    int Score,
    string Summary,
    List<string> Pros,
    List<string> Cons,
    List<string> Skills
);