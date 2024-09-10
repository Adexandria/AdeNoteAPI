using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public class TokenConfiguration
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string TokenSecret { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}
