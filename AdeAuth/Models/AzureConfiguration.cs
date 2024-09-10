using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class AzureConfiguration
    {
        public string Audience { get; set; }
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string Type { get; set; }
        public string NameClaimType { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
