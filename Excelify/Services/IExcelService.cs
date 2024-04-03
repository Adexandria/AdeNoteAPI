using Excelify.Models;
using Excelify.Services.Utility;
using System.Data;


namespace Excelify.Services
{
    public interface IExcelService
    {
        IList<T> ImportToEntity<T> (ISheetImport sheet) where T : class;
        IList<T> ImportToEntity<T> (ISheetImport sheet, IExcelMapper excelifyMapper) where T : class;
        byte[] ExportToBytes<T>(ISheetExport<T> dataExport) where T : class;
        Stream ExportToStream<T>(ISheetExport<T> dataExport) where T : class;
        DataTable ImportToTable(ISheetImport sheet);
        bool CanImportSheet(string extensionType);
    }
}
