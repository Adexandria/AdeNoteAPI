using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IExcel
    {
        Task<ActionResult> ImportEntities(ImportBookDto importBookDto);

        Stream ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities) where T : class;
    }
}
