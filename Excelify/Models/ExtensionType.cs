using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excelify.Models
{
    public enum ExtensionType
    {
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx,
        [Description("application/vnd.ms-excel")]
        xls
    }
}
