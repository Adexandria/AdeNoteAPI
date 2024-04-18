

using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IExportService
    {
        Task<ActionResult<string>> ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities) where T: class;
    }
}
