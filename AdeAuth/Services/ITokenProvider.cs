using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services
{
    public interface ITokenProvider
    {
        string GenerateToken(Dictionary<string, object> claims, int timeInMinutes = 5);

        string GenerateRefreshToken(int tokenSize = 10);

        Dictionary<string,object> ReadToken(string token, bool verifyParameter = false, params string[] claimTypes);

        string GenerateToken(byte[] encodedString);

        int GenerateOTP(byte[] encodedString);

        string[] ReadToken(string token, string delimiter = null);

        bool VerifyOTP(byte[] encodedString, int otp);

        bool VerifyOTP(byte[] encodedString, string otp);

        void SetTokenEncryptionKey(string tokenKey);
    }
}
