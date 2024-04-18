using Excelify.Models;
using Excelify.Services.Utility;
using System.Data;


namespace Excelify.Services
{
    public abstract class ExcelService : IExcelService
    {
        public abstract bool CanImportSheet(string extensionType);
        public abstract byte[] ExportToBytes<T>(ISheetExport<T> dataExport) where T : class;
        public abstract Stream ExportToStream<T>(ISheetExport<T> dataExport) where T : class;
        public abstract DataTable ImportToTable(ISheetImport sheet);
        public abstract IList<T> ImportToEntity<T>(ISheetImport sheet) where T : class;
        public abstract IList<T> ImportToEntity<T>(ISheetImport sheet, IExcelMapper excelifyMapper) where T : class;
    }
}
