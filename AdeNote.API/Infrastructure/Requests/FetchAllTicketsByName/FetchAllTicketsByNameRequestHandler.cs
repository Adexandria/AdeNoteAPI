using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchAllTicketsByName
{
    public class FetchAllTicketsByNameRequestHandler : IRequestHandler<FetchAllTicketsByNameRequest, ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public FetchAllTicketsByNameRequestHandler(ITicketRepository _ticketRepository,
            IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }
        public async Task<ActionResult<PaginatedResponse<TicketsDTO>>> Handle(FetchAllTicketsByNameRequest request, CancellationToken cancellationToken)
        {
            var tickets = ticketRepository.GetTickets(request.Name, request.PageNumber, request.PageSize);

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(request.PageNumber, request.PageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }

        private readonly ITicketRepository ticketRepository;
        private readonly EventConfiguration eventConfiguration;
    }
}
