using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.Excel
{
    public interface IExcel
    {
        Task<ActionResult> ImportEntities(ImportBookDto importBookDto);

        Stream ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities) where T : class;
    }
}
