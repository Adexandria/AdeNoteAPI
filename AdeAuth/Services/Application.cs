
namespace AdeAuth.Services
{
    /// <summary>
    /// Factory to manages password manager, token provider and muilti-factor services
    /// </summary>
    public class Application
    {
        public Application()
        {
           PasswordManager = new PasswordManager();
           TokenProvider = new TokenProvider();
           MfaService = new MfaService();
        }
        /// <summary>
        /// Manages multi-factor service
        /// </summary>
        public IMfaService MfaService { get; set; }

        /// <summary>
        /// Manages password manager
        /// </summary>
        public IPasswordManager PasswordManager { get; set; }

        /// <summary>
        /// Manages token provider
        /// </summary>
        public ITokenProvider TokenProvider { get; set; }
    }
}
