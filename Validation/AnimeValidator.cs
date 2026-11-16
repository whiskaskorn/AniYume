using System.Data;
using DTOs;
using FluentValidation;
namespace Validation;

public class AnimePostValidator : AbstractValidator<AnimePostDTO>
{
    public AnimePostValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Поле не може бути пустим!").Length(1,100).WithMessage("Поле повинно містити від 1 до 100символів!");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Поле не може бути пустим!").Length(1,1000).WithMessage("Поле повинно містити від 1 до 1000символів!");
        RuleFor(x => x.Genres).NotEmpty().WithMessage("Поле не може бути пустим!");
        RuleFor(x => x.ReleaseDate).InclusiveBetween(new DateTime(1900, 1, 1), DateTime.Today);
    }
}