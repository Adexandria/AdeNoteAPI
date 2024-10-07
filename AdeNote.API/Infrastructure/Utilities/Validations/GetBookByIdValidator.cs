using AdeNote.Infrastructure.Requests.GetBookdById;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class GetBookByIdValidator: AbstractValidator<GetBookByIdRequest>
    {
        public GetBookByIdValidator()
        {
            RuleFor(s=>s.UserId).NotEmpty().WithMessage("Invalid user id");

            RuleFor(s=>s.BookId).NotEmpty().WithMessage("Invalid book id");
        }
    }
}
