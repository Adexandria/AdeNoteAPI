using AdeNote.Models.DTOs;
using Excelify.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class ExportEntity<T> : ISheetExport<T> where T : class
    {
        public string SheetName { get; set ; }
        public IList<T> Entities { get; set ; }
    }
}
