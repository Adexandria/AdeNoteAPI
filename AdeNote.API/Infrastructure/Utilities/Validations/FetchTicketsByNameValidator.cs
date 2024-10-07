using AdeNote.Infrastructure.Requests.FetchAllTicketsByName;
using FluentValidation;

namespace AdeNote.Infrastructure.Utilities.Validations
{
    public class FetchTicketsByNameValidator: AbstractValidator<FetchAllTicketsByNameRequest>
    {
        public FetchTicketsByNameValidator()
        {
            RuleFor(s=>s.Name).NotEmpty().WithMessage("Invalid name");
            RuleFor(s => s.PageNumber).GreaterThan(0).WithMessage("Invalid page number");
            RuleFor(s => s.PageSize).GreaterThan(0).WithMessage("Invalid page size");
        }
    }
}
