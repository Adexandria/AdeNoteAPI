using AdeNote.Infrastructure.Requests.GetAllBooks;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class GetAllBooksValidator : AbstractValidator<GetAllBooksRequest>
    {
        public GetAllBooksValidator()
        {
            RuleFor(s=>s.UserId).NotEmpty().WithMessage("Invalid user id");
        }
    }
}
