using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.CreateBooks
{
    public class CreateBooksRequest : IRequest<ActionResult>
    {
       public List<BookCreateDTO> CreateBooks { get; set; }
       
       public Guid UserId { get; set; }
    }
}
