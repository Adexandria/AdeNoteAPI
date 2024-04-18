using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocBuilder.Services
{
    public static class DocFactory
    {
        public static IDocService CreateService()
        {
            return new DocService();
        }
    }
}
