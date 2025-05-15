using FluentValidation;
using UserManagementApplication.Dto;
using UserManagementCore.Types;

namespace UserManagementApplication.Validators;

public class UpdateProfileUserValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileUserValidator()
    {
        RuleFor(x => x.Name)
            .Matches("^[a-zA-Zа-яА-ЯёЁ]+$")
            .WithMessage("Name can contain only Latin and Cyrillic letters.");
        
        RuleFor(x => x.Gender)
            .Must(gender => Enum.IsDefined(typeof(Gender), gender))
            .WithMessage("Invalid gender value.");
        
        RuleFor(x => x.Birthday)
            .Must(birthday => !birthday.HasValue || birthday.Value.Date <= DateTime.UtcNow.Date)
            .WithMessage("Birthday cannot be in the future.");
    }
}