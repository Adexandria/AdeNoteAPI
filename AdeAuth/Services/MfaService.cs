using AdeAuth.Services.Utility;
using Google.Authenticator;

namespace AdeAuth.Services
{
    internal class MfaService : IMfaService
    {
        public MfaService()
        {
            _twoFactorAuthenticator = new TwoFactorAuthenticator();
        }
        public IAuthenticator SetupGoogleAuthenticator(string issuer, string email, byte[] key)
        {
            var authenticator = _twoFactorAuthenticator
                     .GenerateSetupCode(issuer, email, key);

            return new Authenticator(authenticator.QrCodeSetupImageUrl, authenticator.ManualEntryKey);
        }

        public bool VerifyGoogleAuthenticatorTotp(string totp, byte[] key)
        {
            return _twoFactorAuthenticator.ValidateTwoFactorPIN(key, totp);
        }

        public string GenerateGoogleAuthenticatorPin(byte[] key, DateTime expiryDate)
        {
            return _twoFactorAuthenticator.GetCurrentPIN(key, expiryDate);
        }

        public string[] GenerateGoogleAuthenticatorPins(byte[] key, DateTime expiryDate)
        {
            return _twoFactorAuthenticator.GetCurrentPINs(key,expiryDate.TimeOfDay);
        }

        private readonly TwoFactorAuthenticator _twoFactorAuthenticator;
    }
}
