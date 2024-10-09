using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllPages
{
    public class GetAllPagesRequest : IRequest<ActionResult<IEnumerable<PageDTO>>>
    {
        public Guid BookId { get; set; }
    }
}
