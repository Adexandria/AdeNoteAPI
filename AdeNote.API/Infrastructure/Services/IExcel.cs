using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IExcel
    {
        Task<ActionResult> ImportEntities(ImportBookDto importBookDto);

        Task<ActionResult<string>> ExportEntities(Guid userId, string extensionType, string sheetName);
    }
}
