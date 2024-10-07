using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreateLabel
{
    public class CreateLabelRequest : IRequest<ActionResult>
    {
        public string Title { get; set; }
    }
}
