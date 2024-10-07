using AdeNote.Infrastructure.Requests.FetchAllTickets;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class FetchAllTicketValidator : AbstractValidator<FetchAllTicketsRequest>
    {
        public FetchAllTicketValidator()
        {
            RuleFor(s => s.PageNumber).GreaterThan(0).WithMessage("Invalid page number");
            RuleFor(s => s.PageSize).GreaterThan(0).WithMessage("Invalid page size");
        }
    }
}
