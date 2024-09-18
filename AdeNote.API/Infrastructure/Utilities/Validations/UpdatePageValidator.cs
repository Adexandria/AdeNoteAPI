using AdeNote.Infrastructure.Requests.UpdatePage;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class UpdatePageValidator: AbstractValidator<UpdatePageRequest>
    {
        public UpdatePageValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");
            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");
            RuleFor(s => s.PageId).NotEmpty().WithMessage("Invalid page id");
            RuleFor(s => s.UpdatePage)
                .Custom((list, context) => 
                {
                    var isEmpty = string.IsNullOrEmpty(list.Title);
                    if (isEmpty)
                        context.AddFailure("Invalid title");
                });

            RuleFor(s => s.UpdatePage)
                .Custom((list, context) =>
                {
                    var isEmpty = string.IsNullOrEmpty(list.Content);
                    if (isEmpty)
                        context.AddFailure("Invalid content");
                });

        }
    }
}
