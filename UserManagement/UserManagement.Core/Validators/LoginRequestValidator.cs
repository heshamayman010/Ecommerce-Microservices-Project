using FluentValidation;
using UserManagement.Core.DTO;

namespace UserManagement.Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    //Email
    RuleFor(temp => temp.Email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email address format")
      ;

    //Password
    RuleFor(temp => temp.Password)
      .NotEmpty().WithMessage("Password is required")
      ;
  }
}
