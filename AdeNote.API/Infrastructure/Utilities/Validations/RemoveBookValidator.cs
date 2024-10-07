using AdeNote.Infrastructure.Requests.RemoveBook;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class RemoveBookValidator : AbstractValidator<RemoveBookRequest>
    {
        public RemoveBookValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
        }
    }
}
