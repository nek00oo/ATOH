using FluentValidation;
using UserManagementApplication.Dto;

namespace UserManagementApplication.Validators;

public class ChangePasswordUserValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordUserValidator()
    {
        RuleFor(x => x.NewPassword)
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Password must be contain only Latin letters and digits.");
    }
}