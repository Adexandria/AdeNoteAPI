using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IWordService
    {
        Stream ExportToWord<T>(string name, IEnumerable<T> entities, Stream template)
            where T : class;
    }
}
