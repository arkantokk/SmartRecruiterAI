namespace SmartRecruiter.Application.DTOs;

public class CreateJobVacancyRequest
{
    public string Title { get; set; } = string.Empty;
    public string AiPromptTemplate { get; set; } = string.Empty;
}