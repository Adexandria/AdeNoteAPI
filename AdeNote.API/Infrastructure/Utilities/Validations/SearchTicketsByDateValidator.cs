using AdeNote.Infrastructure.Requests.SearchTicketsByDate;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class SearchTicketsByDateValidator : AbstractValidator<SearchTicketsByDateRequest>
    {
        public SearchTicketsByDateValidator()
        {
            RuleFor(s => s.PageNumber).GreaterThan(0).WithMessage("Invalid page number");
            RuleFor(s => s.PageSize).GreaterThan(0).WithMessage("Invalid page size");
            RuleFor(s => s.Created).Matches("(0[1-9]|1[0-2])\\/(0[1-9]|[1-2][0-9]|3[0-1])\\/(20)\\d{2}\\s(0[0-9]|1[0-9]|2[0-3]):(0[0-9]|[1-5][0-9]):(0[0-9]|[1-5][0-9])")
                .WithMessage("nvalid date and time");
        }
    }
}
