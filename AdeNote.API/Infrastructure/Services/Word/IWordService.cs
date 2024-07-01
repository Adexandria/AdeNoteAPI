namespace AdeNote.Infrastructure.Services.Word
{
    public interface IWordService
    {
        Stream ExportToWord<T>(string name, IEnumerable<T> entities, Stream template)
            where T : class;
    }
}
