using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattyPie.Utilities
{
    internal class CosmosConfiguration : ICosmosConfiguration
    {
        public string Endpoint { get; set; }
        public string TokenCredential { get; set; }
        public string DatabaseId { get; set; }
    }
}
