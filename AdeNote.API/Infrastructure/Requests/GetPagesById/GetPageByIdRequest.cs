using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetPagesById
{
    public class GetPageByIdRequest : IRequest<ActionResult<PageDTO>>
    {
        public Guid BookId { get; set; }
        public Guid PageId { get; set; }
    }
}
