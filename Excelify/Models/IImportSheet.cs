using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excelify.Models
{
    public interface IImportSheet
    {
       Stream File { get; set; }
    }
}
