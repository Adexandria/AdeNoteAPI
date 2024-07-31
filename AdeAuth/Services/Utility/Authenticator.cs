using System;


namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Manages the response
    /// </summary>
    internal class Authenticator : IAuthenticator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="qrCodeImage">Qr code </param>
        /// <param name="manualKey">Manual key</param>
        public Authenticator(string qrCodeImage, string manualKey)
        {
           QrCodeImage = qrCodeImage;
           ManualKey = manualKey;
        }

        /// <summary>
        /// Qr code
        /// </summary>
        public string QrCodeImage { get; set; }

        /// <summary>
        /// Manual key from google authenticator
        /// </summary>
        public string ManualKey { get; set; }
    }
}
