using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IBookService 
    { 
        Task<ActionResult> Add(BookCreateDTO createBook);
        Task<ActionResult> Update(Guid bookId,Guid userId,BookUpdateDTO updateBook);
        Task<ActionResult> Remove(Guid bookId,Guid userId);
        Task<ActionResult<IEnumerable<BookDTO>>> GetAll(Guid userId);
        Task<ActionResult<BookDTO>> GetById(Guid bookId,Guid userId);
    }
}
