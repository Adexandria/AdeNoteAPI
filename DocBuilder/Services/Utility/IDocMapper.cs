using System.Data;

namespace DocBuilder.Services.Utility
{
    public interface IDocMapper
    {
        Task<List<T>> Map<T>(IEnumerable<DataRow> rows) where T : class;
    }
}
