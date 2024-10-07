using AdeNote.Infrastructure.Requests.FetchTicketById;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class FetchTicketByIdValidator : AbstractValidator<FetchTicketByIdRequest>
    {
        public FetchTicketByIdValidator()
        {
            RuleFor(s=>s.TicketId).NotEmpty().WithMessage("Invalid ticket id");
        }
    }
}
