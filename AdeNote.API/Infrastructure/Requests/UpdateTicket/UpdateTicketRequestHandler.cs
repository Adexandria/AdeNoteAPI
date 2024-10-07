using AdeMessaging.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using MediatR;
using System.Text.Json;
using AdeAuth.Services.Extensions;

namespace AdeNote.Infrastructure.Requests.UpdateTicket
{
    public class UpdateTicketRequestHandler : IRequestHandler<UpdateTicketRequest, ActionResult>
    {
        public UpdateTicketRequestHandler(ITicketRepository _ticketRepository, IUserRepository _userRepository,
        IBlobService _blobService, IMessagingService _messagingService, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            blobService = _blobService;
            messagingService = _messagingService;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }
        public async Task<ActionResult> Handle(UpdateTicketRequest request, CancellationToken cancellationToken)
        {
            var currentTicket = await ticketRepository.GetTicket(request.TicketId);
            if (currentTicket == null)
                return ActionResult.Failed("There are no existing ticket", StatusCodes.Status400BadRequest);

            var isStatus = Enum.TryParse(request.Status, out Status newStatus);

            if (!isStatus)
            {
                return ActionResult.Failed("Invalid status", StatusCodes.Status400BadRequest);
            }

            currentTicket.Status = newStatus;

            currentTicket.AdminId = request.AdminId;

            currentTicket.SetModifiedDate();

            var commitStatus = await ticketRepository.Update(currentTicket);

            if (!commitStatus)
            {
                return ActionResult.Failed("Failed to update ticket", StatusCodes.Status400BadRequest);
            }

            var admin = await userRepository.GetUser(request.AdminId);


            string message = JsonSerializer.Serialize(new
            {
                status = newStatus.GetDescription(),
                firstName = currentTicket.User.FirstName,
                lastName = currentTicket.User.LastName,
                ticketId = $"tk{currentTicket.Id.ToString("N")[^5..]}",
                emailAddress = currentTicket.User.Email,
                issue = currentTicket.Issue,
                lastUpdated = currentTicket.Modified.ToShortDateString(),
                dateSubmitted = currentTicket.Created.ToShortDateString(),
                agent = admin.FirstName + " " + admin.LastName,
                resolvedDetails = request.SolvedTicketDto?.ResolutionDetails
            });

            messagingService.Publish(message, eventConfiguration.Exchange, eventConfiguration.RoutingKey);

            return ActionResult.SuccessfulOperation();
        }

        private readonly ITicketRepository ticketRepository;
        private readonly IUserRepository userRepository;
        private readonly IBlobService blobService;
        private readonly IMessagingService messagingService;
        private readonly EventConfiguration eventConfiguration;
    }
}
