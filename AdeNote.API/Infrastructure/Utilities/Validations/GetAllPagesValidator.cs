using AdeNote.Infrastructure.Requests.GetAllPages;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class GetAllPagesValidator : AbstractValidator<GetAllPagesRequest>
    {
        public GetAllPagesValidator()
        {
            RuleFor(s=>s.BookId).NotEmpty().WithMessage("Invalid book id");
        }
    }
}
