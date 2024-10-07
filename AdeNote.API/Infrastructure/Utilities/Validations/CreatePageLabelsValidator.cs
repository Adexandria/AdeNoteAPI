using AdeNote.Infrastructure.Requests.CreatePageLabels;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class CreatePageLabelsValidator : AbstractValidator<CreatePageLabelsRequest>
    {
        public CreatePageLabelsValidator()
        {
            RuleFor(s=>s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleForEach(s => s.Labels).NotNull().WithMessage("Invalid labels");
        }
    }
}
