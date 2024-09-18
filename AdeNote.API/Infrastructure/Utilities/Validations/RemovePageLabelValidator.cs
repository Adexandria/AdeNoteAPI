using AdeNote.Infrastructure.Requests.RemovePageLabel;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class RemovePageLabelValidator: AbstractValidator<RemovePageLabelRequest>
    {
        public RemovePageLabelValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s=>s.Title).NotNull().WithMessage("Invalid title");
        }
    }
}
