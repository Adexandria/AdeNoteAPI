using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using MediatR;

namespace AdeNote.Infrastructure.Requests.RemoveTicket
{
    public class RemoveTicketRequestHandler : IRequestHandler<RemoveTicketRequest, ActionResult>
    {
        public RemoveTicketRequestHandler(ITicketRepository _ticketRepository,IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }
        public async Task<ActionResult> Handle(RemoveTicketRequest request, CancellationToken cancellationToken)
        {
            var currentTicket = await ticketRepository.GetTicket(request.TicketId);
            if (currentTicket == null)
                return ActionResult.Failed("There are no existing ticket", StatusCodes.Status400BadRequest);

            var commitStatus = await ticketRepository.Remove(currentTicket);

            if (!commitStatus)
            {
                return ActionResult.Failed("Failed to delete ticket", StatusCodes.Status400BadRequest);
            }

            return ActionResult.SuccessfulOperation();
        }

        private readonly ITicketRepository ticketRepository;
        private readonly EventConfiguration eventConfiguration;
    }
}
