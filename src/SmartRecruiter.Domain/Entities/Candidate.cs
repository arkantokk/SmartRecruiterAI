using SmartRecruiter.Domain.Enums;
using SmartRecruiter.Domain.ValueObjects;

namespace SmartRecruiter.Domain.Entities;

public class Candidate
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? ResumeUrl { get; private set; }
    public CandidateStatus Status { get; private set; }
    public CandidateEvaluation? Evaluation { get; private set; }
    private readonly List<string> _skills = [];
    public IReadOnlyCollection<string> Skills => _skills.AsReadOnly();
    public Guid JobVacancyId { get; private set; }
    
    public Candidate(string firstName, string lastName, string email, Guid jobVacancyId)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentNullException(nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentNullException(nameof(lastName));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
        if (jobVacancyId == Guid.Empty) throw new ArgumentException("Candidate must be assigned to a vacancy", nameof(jobVacancyId));
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        JobVacancyId = jobVacancyId;
        Status = CandidateStatus.Applied;
    }

    public void SetResume(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
        ResumeUrl = url;
    }

    public void UpdateAssessment(CandidateEvaluation evaluation)
    {
        Evaluation = evaluation ?? throw new ArgumentNullException(nameof(evaluation));
        Status = CandidateStatus.Screening;
    }

    public void AddSkill(string skill)
    {
        if (string.IsNullOrWhiteSpace(skill)) return;

        var cleanSkill = skill.Trim();
        if (!_skills.Contains(cleanSkill))
        {
            _skills.Add(cleanSkill);
        }
    }

    public void Evaluate(int score, string summary, List<string> pros, List<string> cons, List<string> skills)
    {
        Evaluation = new CandidateEvaluation(score, summary, pros, cons, skills);
        Status = CandidateStatus.Screening;
    }
    
    public void ApplyAiAnalysis(
        string? extractedFirstName, 
        string? extractedLastName, 
        int score, 
        string summary, 
        List<string> pros, 
        List<string> cons, 
        List<string> skills)
    {
        if (!string.IsNullOrWhiteSpace(extractedFirstName))
            FirstName = extractedFirstName;

        if (!string.IsNullOrWhiteSpace(extractedLastName))
            LastName = extractedLastName;
        
        Evaluation = new CandidateEvaluation(score, summary, pros, cons, skills);
        
        _skills.Clear();
        if (skills != null)
        {
            _skills.AddRange(skills);
        }
        
        Status = CandidateStatus.Screening;
    }
}