

namespace Excelify.Models
{
    public interface IEntityExport<TEntity> 
    {
       string SheetName { get; set; }
       IList<TEntity> Entities { get; set; }
    }
}
