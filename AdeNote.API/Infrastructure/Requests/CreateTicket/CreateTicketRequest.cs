using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreateTicket
{
    public class CreateTicketRequest : IRequest<ActionResult>
    {
        public string Issue { get; set; }

        public string Description { get; set; }

        public Stream Image { get; set; }

        public string Email { get; set; }

    }
}
