using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services
{
    public interface ITokenProvider
    {
        string GenerateToken(Dictionary<string, object> claims, int timeInMinutes = 5);

        string GenerateRefreshToken(int tokenSize = 10);

        IList<string> VerifyToken(string token, bool verifyParameter = false, params string[] claimTypes);

        void GetTokenEncryptionKey(string tokenKey);
    }
}
