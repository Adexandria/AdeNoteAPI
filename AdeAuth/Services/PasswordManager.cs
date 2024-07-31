using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdeAuth.Services.Interfaces;

namespace AdeAuth.Services
{
    /// <summary>
    /// Manages password manager
    /// </summary>
    class PasswordManager : IPasswordManager
    {
        /// <summary>
        /// Hashes password
        /// </summary>
        /// <param name="password">password to hash</param>
        /// <param name="salt">Key to encode password</param>
        /// <returns></returns>
        public string HashPassword(string password, out string salt)
        {
            salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password,salt);
            return hash;
        }

        /// <summary>
        /// Verifies password with current password
        /// </summary>
        /// <param name="password">Password to hash</param>
        /// <param name="currentPassword">Current password to verify</param>
        /// <param name="salt">Key to encode password</param>
        /// <returns></returns>
        public bool VerifyPassword(string password, string currentPassword, string salt)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password,salt);
            return currentPassword.Equals(hashedPassword);
        }
    }
}
