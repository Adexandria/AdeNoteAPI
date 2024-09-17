using AdeNote.Infrastructure.Requests.UpdateBook;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class UpdateBookValidator: AbstractValidator<UpdateBookRequest>
    {
        public UpdateBookValidator()
        {
            RuleFor(s => s.UserId).NotEmpty().WithMessage("Invalid user id");

            RuleFor(s => s.BookId).NotEmpty().WithMessage("Invalid book id");

            RuleFor(s => s.UpdateBook)
             .Custom((list, context) =>
             {
                 var isEmpty = string.IsNullOrEmpty(list.Title);
                 if (isEmpty)
                     context.AddFailure("Invalid title");
             });

            RuleFor(s => s.UpdateBook)
               .Custom((list, context) =>
               {
                   var isEmpty = string.IsNullOrEmpty(list.Description);
                   if (isEmpty)
                       context.AddFailure("Invalid description");
               });
        }
    }
}
