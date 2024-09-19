using AdeNote.Infrastructure.Requests.CreateTicket;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class CreateTicketValidator: AbstractValidator<CreateTicketRequest>
    {
        public CreateTicketValidator()
        {
            RuleFor(s=>s.Email).NotNull().NotEmpty().WithMessage("Invalid email");
            RuleFor(s => s.Issue).NotEmpty().WithMessage("Invalid issue");
            RuleFor(s => s.Description).NotEmpty().WithMessage("Invalid description");
        }
    }
}
