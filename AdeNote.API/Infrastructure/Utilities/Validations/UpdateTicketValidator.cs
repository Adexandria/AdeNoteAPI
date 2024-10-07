using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Requests.UpdateTicket;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class UpdateTicketValidator :  AbstractValidator<UpdateTicketRequest>
    {
        public UpdateTicketValidator()
        {
            RuleFor(s=>s.TicketId).NotEmpty().WithMessage("Invalid ticket id");
            RuleFor(s=>s.Status).NotEmpty().IsValidTicketStatus().WithMessage("Invalid status");
            RuleFor(s => s.AdminId).NotEmpty().WithMessage("Invalid admin id");
        }
    }
}
