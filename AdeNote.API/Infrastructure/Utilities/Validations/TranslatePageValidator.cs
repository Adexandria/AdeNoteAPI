using AdeNote.Infrastructure.Requests.TranslatePage;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class TranslatePageValidator : AbstractValidator<TranslatePageRequest>
    {
        public TranslatePageValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s => s.TranslatedLanguage).NotNull()
                .WithMessage("Invalid translated lanaguage");
        }
    }
}
