using FluentValidation;
using StudentRegistrationSystem.DTOs;

namespace StudentRegistrationSystem.Validators;

public class StudentValidator : AbstractValidator<StudentDto>
{
    public StudentValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Course)
            .NotEmpty();
    }
}