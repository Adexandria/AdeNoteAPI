using Excelify.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class ImportSheet : IImportSheet
    {
        public ImportSheet(Stream file, int sheetName)
        {
            File = file;
            SheetName = sheetName;
        }
        public Stream File { get; set; }
        public int SheetName { get; set; }
    }
}
