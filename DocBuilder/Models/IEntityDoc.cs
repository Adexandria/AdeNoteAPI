using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocBuilder.Models
{
    public interface IEntityDoc<T>: IBaseDoc
    {
        public IList<T> Entities { get; set; }
    }
}
