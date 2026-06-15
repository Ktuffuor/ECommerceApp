using System.Net.Mail;
using FluentValidation;

namespace Application.Features.Users.Commands;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(BeAValidEmailAddress).WithMessage("A valid email format is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }

    private static bool BeAValidEmailAddress(string email)
    {
        var trimmedEmail = email.Trim();

        return MailAddress.TryCreate(trimmedEmail, out var mailAddress)
               && string.Equals(mailAddress.Address, trimmedEmail, StringComparison.OrdinalIgnoreCase);
    }
}
