using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface ILabelService
    {
        Task<ActionResult> Add(LabelCreateDTO createLabel);
        Task<ActionResult> Update(Guid labelId, LabelUpdateDTO createLabel);
        Task<ActionResult> Remove(Guid labelId);
        Task<ActionResult<LabelDTO>> GetById(Guid labelId);
        Task<ActionResult<IEnumerable<LabelDTO>>> GetAll();
    }
}
