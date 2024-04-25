

namespace AdeAuth.Services
{
    public class AuthFactory
    {
        public static Application CreateService()
        {
            return new Application();
        }
    }
}
