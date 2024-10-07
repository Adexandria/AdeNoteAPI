using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.SearchTickets
{
    public class SearchTicketsRequestHandler : IRequestHandler<SearchTicketsRequest, ActionResult<PaginatedResponse<TicketsDTO>>>
    {
        public SearchTicketsRequestHandler(ITicketRepository _ticketRepository, IUserRepository _userRepository,
        IBlobService _blobService, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            blobService = _blobService;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }

        public async Task<ActionResult<PaginatedResponse<TicketsDTO>>> Handle(SearchTicketsRequest request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse(request.Status, out Status newStatus))
            {
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("Invalid status", StatusCodes.Status400BadRequest);
            }

            var tickets = ticketRepository.SearchTickets(x => x.Status == newStatus, request.PageNumber, request.PageSize).ToList();

            if (!tickets.Any())
                return ActionResult<PaginatedResponse<TicketsDTO>>.Failed("There are no tickets", StatusCodes.Status400BadRequest);

            var currentTickets = tickets.Map<IEnumerable<Ticket>, IEnumerable<TicketsDTO>>(MappingService.TicketsConfig());

            var paginatedResponse = new PaginatedResponse<TicketsDTO>(request.PageNumber, request.PageSize, currentTickets);

            return ActionResult<PaginatedResponse<TicketsDTO>>.SuccessfulOperation(paginatedResponse);
        }

        private readonly ITicketRepository ticketRepository;
        private readonly IUserRepository userRepository;
        private readonly IBlobService blobService;
        private readonly EventConfiguration eventConfiguration;
    }
}
