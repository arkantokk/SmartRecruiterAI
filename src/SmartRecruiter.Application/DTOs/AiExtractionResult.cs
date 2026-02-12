namespace SmartRecruiter.Application.DTOs;

public record AiExtractionResult(
    string FirstName,
    string LastName,
    int Score,
    string Summary,
    List<string> Pros,
    List<string> Cons,
    List<string> Skills
);