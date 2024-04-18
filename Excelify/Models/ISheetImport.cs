
namespace Excelify.Models
{
    /// <summary>
    /// Model used to import sheet 
    /// </summary>
    public interface ISheetImport
    {
        /// <summary>
        /// File to import
        /// </summary>
       Stream File { get; set; }

        /// <summary>
        /// Sheet name to import
        /// </summary>
       int SheetName { get; set; }
    }
}
