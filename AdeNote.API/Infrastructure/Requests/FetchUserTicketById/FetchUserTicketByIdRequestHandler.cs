using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Automappify.Services;
using MediatR;

namespace AdeNote.Infrastructure.Requests.FetchUserTicketById
{
    public class FetchUserTicketByIdRequestHandler : IRequestHandler<FetchUserTicketByIdRequest, ActionResult<UserTicketDto>>
    {
        public FetchUserTicketByIdRequestHandler(ITicketRepository _ticketRepository,
                IUserRepository _userRepository, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }

        public async Task<ActionResult<UserTicketDto>> Handle(FetchUserTicketByIdRequest request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.GetTicket(request.TicketId);
            if (ticket == null)
                return ActionResult<UserTicketDto>.Failed("There are no existing tickets", StatusCodes.Status400BadRequest);

            var ticketAdmin = await userRepository.GetUser(ticket.AdminId.GetValueOrDefault());

            var currentTicket = ticket.Map<Ticket, UserTicketDto>(MappingService.UserTicketConfig());

            currentTicket.TicketId = request.TicketId;

            currentTicket.Map(ticketAdmin, MappingService.TicketAdminConfig());

            return ActionResult<UserTicketDto>.SuccessfulOperation(currentTicket);
        }

         private readonly ITicketRepository ticketRepository;
         private readonly IUserRepository userRepository;
         private readonly EventConfiguration eventConfiguration;
    }
}
