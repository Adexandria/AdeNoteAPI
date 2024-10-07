using AdeNote.Infrastructure.Requests.RemoveAllPageLabels;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class RemoveAllPageLabelsValidator : AbstractValidator<RemoveAllPageLabelsRequest>
    {
        public RemoveAllPageLabelsValidator()
        {
            RuleFor(s=>s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
        }
    }
}
