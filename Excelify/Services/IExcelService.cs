using Excelify.Models;
using Excelify.Services.Utility;
using System.Data;


namespace Excelify.Services
{
    public interface IExcelService
    {
        IList<T> ImportToEntity<T> (IImportSheet sheet) where T : class;
        IList<T> ImportToEntity<T> (IImportSheet sheet, IExcelMapper excelifyMapper) where T : class;
        byte[] ExportToBytes<T>(IEntityExport<T> dataExport) where T : class;
        Stream ExportToStream<T>(IEntityExport<T> dataExport) where T : class;
        DataTable ImportToTable(IImportSheet sheet);
        bool CanImportSheet(string extensionType);
    }
}
