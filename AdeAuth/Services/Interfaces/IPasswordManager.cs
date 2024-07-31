namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages password manager
    /// </summary>
    public interface IPasswordManager
    {
        /// <summary>
        /// Hashes password
        /// </summary>
        /// <param name="password">password to hash</param>
        /// <param name="salt">Key to encode password</param>
        /// <returns></returns>
        string HashPassword(string password, out string salt);

        /// <summary>
        /// Verifies password with current password
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="currentPassword">Current password to verify</param>
        /// <param name="salt">Key to encode password</param>
        /// <returns></returns>
        bool VerifyPassword(string password, string currentPassword, string salt);
    }
}
