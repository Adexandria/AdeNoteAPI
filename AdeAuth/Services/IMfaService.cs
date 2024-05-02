using AdeAuth.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services
{
    public interface IMfaService
    {
        IAuthenticator SetupGoogleAuthenticator(string issuer, string email, byte[] key);

        bool VerifyGoogleAuthenticatorTotp(string totp, byte[] key);

        string GenerateGoogleAuthenticatorPin(byte[] key, DateTime expiryDate);

        string[] GenerateGoogleAuthenticatorPins(byte[] key, DateTime expiryDate);
    }
}
