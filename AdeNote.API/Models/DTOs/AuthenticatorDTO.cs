namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// Displays the authenticator details
    /// </summary>
    public class AuthenticatorDTO
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="manualKey">A manual key</param>
        /// <param name="qrCodeUrl">Qr code url</param>
        public AuthenticatorDTO(string manualKey,string qrCodeUrl)
        {
            ManualKey = manualKey;
            QRCodeUrl = qrCodeUrl;
        }
        /// <summary>
        /// Manual key
        /// </summary>
        public string ManualKey { get; set; }

        /// <summary>
        /// Qr code url
        /// </summary>
        public string QRCodeUrl { get; set; }
    }
}
