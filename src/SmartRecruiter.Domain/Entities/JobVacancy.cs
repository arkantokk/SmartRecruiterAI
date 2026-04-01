namespace SmartRecruiter.Domain.Entities;

public class JobVacancy
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string AiPromptTemplate { get; private set; }
    public string UserId { get; private set; }
    public JobVacancy(string title, string aiPromptTemplate, string userId)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentNullException(nameof(title));
        if (string.IsNullOrWhiteSpace(aiPromptTemplate)) throw new ArgumentNullException(nameof(aiPromptTemplate));

        Id = Guid.NewGuid();
        Title = title;
        AiPromptTemplate = aiPromptTemplate;
        UserId = userId;
    }

    public void UpdateJobVacancy(string title, string newTemplate)
    {
        if (string.IsNullOrWhiteSpace(title)) 
            throw new ArgumentException("Title cannot be empty");
        if (string.IsNullOrWhiteSpace(newTemplate)) throw new ArgumentNullException(nameof(newTemplate));
        Title = title;
        AiPromptTemplate = newTemplate;
    }
}