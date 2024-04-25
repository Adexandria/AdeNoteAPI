using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services
{
    class PasswordManager : IPasswordManager
    {
        public string HashPassword(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return hash;
        }

        public bool VerifyPassword(string password, string currentPassword)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return currentPassword.Equals(hashedPassword);
        }
    }
}
