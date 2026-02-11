using FluentValidation;
using SmartRecruiter.Application.DTOs;

namespace SmartRecruiter.Application.Validators;

public class CreateCandidateValidator : AbstractValidator<CreateCandidateRequest>
{
    public CreateCandidateValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Name can't be empty")
            .MaximumLength(50).WithMessage("Name can't be too long");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last email can't be empty");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Enter correct email");

        RuleFor(x => x.JobVacancyId)
            .NotEmpty().WithMessage("Vacancy id is mandatory");
    }
}