using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Requests.SearchTickets;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class SearchTicketsValidator :  AbstractValidator<SearchTicketsRequest>
    {
        public SearchTicketsValidator()
        {
            RuleFor(s => s.Status).IsValidTicketStatus().WithMessage("Invalid status");
            RuleFor(s => s.PageNumber).GreaterThan(0).WithMessage("Invalid page number");
            RuleFor(s => s.PageSize).GreaterThan(0).WithMessage("Invalid page size");
        }
    }
}
