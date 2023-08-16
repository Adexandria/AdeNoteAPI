namespace AdeNote.Models.DTOs
{
    public class AuthenticatorDTO
    {
        public AuthenticatorDTO(string manualKey,string qrCodeUrl)
        {
            ManualKey = manualKey;
            QRCodeUrl = qrCodeUrl;
        }
        public string ManualKey { get; set; }
        public string QRCodeUrl { get; set; }
    }
}
