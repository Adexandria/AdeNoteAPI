using AdeNote.Infrastructure.Requests.CreateBooks;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class CreateBooksValidator: AbstractValidator<CreateBooksRequest>
    {
        public CreateBooksValidator()
        {
            RuleFor(s => s.UserId).NotNull().WithMessage("Invalid user id");

            RuleFor(s => s.CreateBooks).NotNull().WithMessage("Request body cannot be null");

            RuleForEach(s => s.CreateBooks)
                .Custom((list, context) =>
                {
                    var isEmpty =  string.IsNullOrEmpty(list.Title);
                    if (isEmpty)
                        context.AddFailure("Invalid title");
                });

            RuleForEach(s => s.CreateBooks)
               .Custom((list, context) =>
               {
                   var isEmpty = string.IsNullOrEmpty(list.Description);
                   if (isEmpty)
                       context.AddFailure("Invalid description");
               });
        }
    }
}
