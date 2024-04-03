namespace Excelify.Models
{
    /// <summary>
    /// Model used to export entities to excel sheet
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface ISheetExport<TEntity> 
    {
        /// <summary>
        /// Sheet name
        /// </summary>
       string SheetName { get; set; }

        /// <summary>
        /// List of entities to export
        /// </summary>
       IList<TEntity> Entities { get; set; }
    }
}
