using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchTicketById
{
    public class FetchTicketByIdRequestHandler : IRequestHandler<FetchTicketByIdRequest, ActionResult<TicketDTO>>
    {

        public FetchTicketByIdRequestHandler(ITicketRepository _ticketRepository, IUserRepository _userRepository,
        IBlobService _blobService, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            blobService = _blobService;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }
        public async Task<ActionResult<TicketDTO>> Handle(FetchTicketByIdRequest request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.GetTicket(request.TicketId);
            if (ticket == null)
                return ActionResult<TicketDTO>.Failed("There are no existing tickets", StatusCodes.Status400BadRequest);

            var ticketAdmin = await userRepository.GetUser(ticket.AdminId.GetValueOrDefault());

            var currentTicket = ticket.Map<Ticket, TicketDTO>(MappingService.TicketConfig());

            currentTicket.Map(ticketAdmin, MappingService.TicketAdminConfig());

            return ActionResult<TicketDTO>.SuccessfulOperation(currentTicket);
        }

        private readonly ITicketRepository ticketRepository;
        private readonly IUserRepository userRepository;
        private readonly IBlobService blobService;
        private readonly EventConfiguration eventConfiguration;
    }
}
