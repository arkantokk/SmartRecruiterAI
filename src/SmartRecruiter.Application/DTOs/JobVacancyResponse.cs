namespace SmartRecruiter.Application.DTOs;

public record JobVacancyResponse(
    Guid Id,
    string Title,
    string AiPromptTemplate
    );