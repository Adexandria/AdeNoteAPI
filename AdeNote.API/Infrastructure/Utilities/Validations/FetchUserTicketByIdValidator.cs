using AdeNote.Infrastructure.Requests.FetchUserTicketById;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class FetchUserTicketByIdValidator : AbstractValidator<FetchUserTicketByIdRequest>
    {
        public FetchUserTicketByIdValidator()
        {
            RuleFor(s => s.TicketId).NotEmpty().WithMessage("Invalid ticket id");
        }
    }
}
