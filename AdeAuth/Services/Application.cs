
namespace AdeAuth.Services
{
    public class Application
    {
        public Application()
        {
           PasswordManager = new PasswordManager();
           TokenProvider = new TokenProvider();
           MfaService = new MfaService();
        }
        public IMfaService MfaService { get; set; }
        public IPasswordManager PasswordManager { get; set; }
        public ITokenProvider TokenProvider { get; set; }
    }
}
