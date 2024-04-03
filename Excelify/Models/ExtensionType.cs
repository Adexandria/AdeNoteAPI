using System.ComponentModel;

namespace Excelify.Models
{
    /// <summary>
    /// Specifies the extension type of the file
    /// </summary>
    public enum ExtensionType
    {
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx,

        [Description("application/vnd.ms-excel")]
        xls,

        [Description("text/csv")]
        csv
    }
}
