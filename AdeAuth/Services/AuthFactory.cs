

namespace AdeAuth.Services
{
    /// <summary>
    /// Factory design pattern to create services
    /// </summary>
    public class AuthFactory
    {
        /// <summary>
        /// Create services
        /// </summary>
        /// <returns></returns>
        public static Application CreateService()
        {
            return new Application();
        }
    }
}
