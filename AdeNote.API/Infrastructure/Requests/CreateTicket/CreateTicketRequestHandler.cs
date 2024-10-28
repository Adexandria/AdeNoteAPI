using AdeMessaging.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EmailSettings;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Models;
using MediatR;
using Automappify.Services;

namespace AdeNote.Infrastructure.Requests.CreateTicket
{
    public class CreateTicketRequestHandler : IRequestHandler<CreateTicketRequest, ActionResult>
    {

        public CreateTicketRequestHandler(ITicketRepository _ticketRepository, IUserRepository _userRepository,
    IBlobService _blobService
    , IMessagingService _messagingService, IConfiguration config)
        {
            ticketRepository = _ticketRepository;
            userRepository = _userRepository;
            blobService = _blobService;
            eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>() ?? throw new NullReferenceException(nameof(eventConfiguration));

        }
        public async Task<ActionResult> Handle(CreateTicketRequest request, CancellationToken cancellationToken)
        {
            var currentUser = await userRepository.GetUserByEmail(request.Email);

            if (currentUser == null)
            {
                return ActionResult.Failed("Invalid user", StatusCodes.Status400BadRequest);
            }

            var ticket = request.Map<CreateTicketRequest, Ticket>(MappingService.TicketStreamConfig());

            ticket.Issuer = currentUser.Id;

            if (request.Image?.Length != 0 || request.Image == null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var url = await blobService.UploadImage($"T{Guid.NewGuid().ToString()[..4]}", request.Image, cancellationToken) ?? $"T{Guid.NewGuid().ToString()[..4]}";

                if (string.IsNullOrEmpty(url))
                {
                    return ActionResult<string>.Failed(url, 400);
                }

                ticket.ImageUrl = url;
            }

            var commitStatus = await ticketRepository.Add(ticket);

            if (!commitStatus)
                return ActionResult.Failed("Failed to create a ticket");

            return ActionResult.SuccessfulOperation();
        }


        private readonly ITicketRepository ticketRepository;
        private readonly IUserRepository userRepository;
        private readonly IBlobService blobService;
        private readonly EventConfiguration eventConfiguration;
    }
}
