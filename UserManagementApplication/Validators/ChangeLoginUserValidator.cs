using FluentValidation;
using UserManagementApplication.Dto;

namespace UserManagementApplication.Validators;

public class ChangeLoginUserValidator : AbstractValidator<ChangeLoginDto>
{
    public ChangeLoginUserValidator()
    {
        RuleFor(x => x.NewLogin)
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Login can contain only Latin letters and digits.");
    }
}