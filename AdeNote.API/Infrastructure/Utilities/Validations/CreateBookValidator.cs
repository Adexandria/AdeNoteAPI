using AdeNote.Infrastructure.Requests.CreateBook;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class CreateBookValidator : AbstractValidator<CreateBookRequest>
    {
        public CreateBookValidator()
        {
           RuleFor(x => x.Title)
                .NotNull().WithMessage("Invalid title");

            RuleFor(x => x.Description).NotNull().WithMessage("Invalid description");

            RuleFor(x => x.UserId).NotNull().WithMessage("Invalid user id");
        }
    }
}
