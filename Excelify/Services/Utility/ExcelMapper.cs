using System.Data;

namespace Excelify.Services.Utility
{
    public abstract class ExcelMapper : IExcelMapper
    {
        public abstract Task<List<T>> Map<T>(IEnumerable<DataRow> rows) where T: class;
    }
}
