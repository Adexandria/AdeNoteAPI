using AdeNote.Infrastructure.Requests.RemovePage;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class RemovePageValidator : AbstractValidator<RemovePageRequest>
    {
        public RemovePageValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
        }
    }
}
