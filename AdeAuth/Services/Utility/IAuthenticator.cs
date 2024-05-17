using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    public interface IAuthenticator
    {
        public string QrCodeImage { get; set; }

        public string ManualKey { get; set; }
    }
}
