using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Manages authenticator
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Qr code 
        /// </summary>
        public string QrCodeImage { get; set; }

        /// <summary>
        /// Manual key
        /// </summary>
        public string ManualKey { get; set; }
    }
}
