
namespace AdeAuth.Services
{
    public class Application
    {
        public Application()
        {
           PasswordManager = new PasswordManager();
           TokenProvider = new TokenProvider();
        }
        public IPasswordManager PasswordManager { get; set; }
        public ITokenProvider TokenProvider { get; set; }
    }
}
