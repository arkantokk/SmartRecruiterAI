namespace SmartRecruiter.Domain.Entities;

public class JobVacancy
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string AiPromptTemplate { get; private set; }
    
    public JobVacancy(string title, string aiPromptTemplate)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
        if (string.IsNullOrWhiteSpace(aiPromptTemplate)) throw new ArgumentNullException(nameof(aiPromptTemplate));

        Id = Guid.NewGuid();
        Title = title;
        AiPromptTemplate = aiPromptTemplate;
    }

    public void UpdateAiPrompt(string newTemplate)
    {
        if (string.IsNullOrWhiteSpace(newTemplate)) throw new ArgumentNullException(nameof(newTemplate));
        AiPromptTemplate = newTemplate;
    }
}