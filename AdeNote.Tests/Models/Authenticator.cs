using AdeAuth.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeNote.Tests.Models
{
    internal class Authenticator : IAuthenticator
    {
        public Authenticator(string qrCodeImage, string manualKey)
        {
            QrCodeImage = qrCodeImage;
            ManualKey = manualKey;
        }
        public string QrCodeImage { get; set; }
        public string ManualKey { get; set; }
    }
}
