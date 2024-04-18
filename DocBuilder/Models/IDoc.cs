using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocBuilder.Models
{
    public interface IDoc : IBaseDoc
    {
        public string[] Texts { get; set; }
    }
}
