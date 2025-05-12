using FluentValidation;
using UserManagement.Application.UseCases.Auth;
namespace UserManagement.Application.Common.Validation;

public class LoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}
