using AdeNote.Models.DTOs;
using Excelify.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class ExportEntity : ISheetExport<BookDTO>
    {
        public string SheetName { get; set ; }
        public IList<BookDTO> Entities { get; set ; }
    }
}
