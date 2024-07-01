using Excelify.Models;

namespace AdeNote.Infrastructure.Utilities.ExcelSettings
{
    public class ImportSheet : ISheetImport
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
