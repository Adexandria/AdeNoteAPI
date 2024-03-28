using Excelify.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class ImportSheet : IImportSheet
    {
        public ImportSheet(Stream file)
        {
            File = file;
        }
        public Stream File { get; set; }
    }
}
