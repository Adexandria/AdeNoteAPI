using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IPageService
    {
        Task<ActionResult> Add(Guid bookId,Guid userId,PageCreateDTO createPage);
        Task<ActionResult> Update(Guid bookId, Guid userId, Guid pageId, PageUpdateDTO updatePage);
        Task<ActionResult> Remove(Guid bookId, Guid userId, Guid pageId);
        Task<ActionResult<IEnumerable<PageDTO>>> GetAll(Guid bookId);
        Task<ActionResult<PageDTO>> GetById(Guid bookId, Guid pageId);
    }
}
