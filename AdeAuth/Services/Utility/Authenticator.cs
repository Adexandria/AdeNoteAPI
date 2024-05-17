using System;


namespace AdeAuth.Services.Utility
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
