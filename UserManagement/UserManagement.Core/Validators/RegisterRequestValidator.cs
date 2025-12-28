using FluentValidation;
using UserManagement.Core.DTO;

namespace UserManagement.Core.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
  public RegisterRequestValidator()
  {

    RuleFor(x => x.Email).EmailAddress().WithMessage("this must be an email address")
    .NotEmpty().NotNull().WithMessage("cant be null or empty space ");

    RuleFor(x => x.PersonName)
    .NotEmpty().NotNull().WithMessage("cant be null or empty space ");

    RuleFor(x => x.Password)
    .NotEmpty().NotNull().WithMessage("cant be null or empty space ").MaximumLength(50).WithMessage("cant be more than 50 char");

  }



}