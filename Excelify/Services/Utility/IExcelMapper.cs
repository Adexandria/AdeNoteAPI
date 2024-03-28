using System.Data;

namespace Excelify.Services.Utility
{
    public interface IExcelMapper
    {
        Task<List<T>> Map<T>(IEnumerable<DataRow> rows) where T : class ;
    }
}
