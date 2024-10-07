using AdeNote.Infrastructure.Requests.InsertVideo;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class InsertVideoValidator : AbstractValidator<InsertVideoRequest>
    {
        public InsertVideoValidator()
        {
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s => s.Description).NotEmpty().WithMessage("Invalid description");
        }
    }
}
