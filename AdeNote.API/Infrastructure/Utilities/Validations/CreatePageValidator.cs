using AdeNote.Infrastructure.Requests.CreatePage;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class CreatePageValidator : AbstractValidator<CreatePageRequest>
    {
        public CreatePageValidator()
        {
            RuleFor(s=>s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.Title).NotNull().WithMessage("Invalid title");
            RuleFor(s => s.Content).NotNull().WithMessage("Invalid content");
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Inavlid user id");
        }
    }
}
