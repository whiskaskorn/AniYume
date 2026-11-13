using FluentValidation;
using DTOs;

namespace Validation;

public class UserRegistrationValidator : AbstractValidator<UserRegistrationDTO>
{
    public UserRegistrationValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Поле не може бути пустим!").Length(5,30).WithMessage("Поле повинно містити від 5 до 30 символів!");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Поле не може бути пустим!").Length(7,50).WithMessage("Поле повинно містити від 7 до 50 символів!");
    }
}

public class UserLoginValidator : AbstractValidator<UserLoginDTO>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Поле не може бути пустим!").Length(7,50).WithMessage("Поле повинно містити від 7 до 50 символів!");
    }
}

public class UserConfirmEmailValidator : AbstractValidator<UserConfirmEmailDTO>
{
    public UserConfirmEmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Поле не може бути пустим!").MaximumLength(255).WithMessage("Поле занадто довге!").EmailAddress().WithMessage("Некоректний формат email!");
        RuleFor(x => x.Code).GreaterThan(0).WithMessage("Поле повинно містити 6 символів!");
    }
}