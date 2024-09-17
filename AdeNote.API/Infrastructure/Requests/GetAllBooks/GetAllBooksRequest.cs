using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.GetAllBooks
{
    public class GetAllBooksRequest : IRequest<ActionResult<IEnumerable<BookDTO>>>
    {
        public Guid UserId { get; set; }
    }
}
