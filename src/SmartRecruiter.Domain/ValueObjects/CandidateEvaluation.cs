namespace SmartRecruiter.Domain.ValueObjects;

public record CandidateEvaluation(
    int Score,
    string Summary,
    List<string> Pros,
    List<string> Cons
    );