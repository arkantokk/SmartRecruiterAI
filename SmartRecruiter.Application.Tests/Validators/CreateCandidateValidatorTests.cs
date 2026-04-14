using FluentValidation;
using FluentValidation.TestHelper;
using SmartRecruiter.Application.DTOs;
using SmartRecruiter.Application.Validators;

namespace SmartRecruiter.Application.Tests.Validators;

public class CreateCandidateValidatorTests
{
    private readonly CreateCandidateValidator _validator;
    
    public CreateCandidateValidatorTests()
    {
        _validator = new CreateCandidateValidator();
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var request = new CreateCandidateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            JobVacancyId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_Have_Error_When_FirstName_Is_Empty(string invalidFirstName)
    {
        var request = new CreateCandidateRequest
        {
            FirstName = invalidFirstName,
            LastName = "",
            Email = "john.doe@example.com",
            JobVacancyId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("Name can't be empty");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var request = new CreateCandidateRequest
        {
            FirstName = "Name",
            LastName = "lastName",
            Email = "john.doe",
            JobVacancyId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Enter correct email");
    }
}