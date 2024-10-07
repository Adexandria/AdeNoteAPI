using AdeNote.Infrastructure.Requests.GetPagesById;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class GetPageByIdValidator : AbstractValidator<GetPageByIdRequest>
    {
        public GetPageByIdValidator()
        {
            RuleFor(s=>s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s=>s.BookId).NotEmpty().WithMessage("Invalid book id");
        }
    }
}
