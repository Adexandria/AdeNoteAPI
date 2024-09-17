using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using MediatR;

namespace AdeNote.Infrastructure.Requests.UpdateBook
{
    public class UpdateBookRequest: IRequest<ActionResult>
    {
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public BookUpdateDTO UpdateBook { get; set; }
    }
}
