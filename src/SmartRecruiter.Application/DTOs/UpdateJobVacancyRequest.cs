namespace SmartRecruiter.Application.DTOs;

public record UpdateJobVacancyRequest(
    string Title,
    string AiPromptTemplate
    );