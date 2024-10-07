using AdeNote.Infrastructure.Utilities;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllPages
{
    public class GetAllPagesRequest : IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
    }
}
