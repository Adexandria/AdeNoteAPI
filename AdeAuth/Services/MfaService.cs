using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Google.Authenticator;

namespace AdeAuth.Services
{
    /// <summary>
    /// Manages multi-factor service
    /// </summary>
    internal class MfaService : IMfaService
    {
        public MfaService()
        {
            _twoFactorAuthenticator = new TwoFactorAuthenticator();
        }

        /// <summary>
        /// Sets up google authenticator 
        /// </summary>
        /// <param name="issuer">The organisation issuing the service</param>
        /// <param name="email">The email of the user</param>
        /// <param name="key">The encoded key</param>
        /// <returns></returns>
        public IAuthenticator SetupGoogleAuthenticator(string issuer, string email, byte[] key)
        {
            var authenticator = _twoFactorAuthenticator
                     .GenerateSetupCode(issuer, email, key);

            return new Authenticator(authenticator.QrCodeSetupImageUrl, authenticator.ManualEntryKey);
        }

        /// <summary>
        /// Verifies google authenticator one time password
        /// </summary>
        /// <param name="totp">Time-based one time password</param>
        /// <param name="key">The encoded key</param>
        /// <returns>boolean value</returns>
        public bool VerifyGoogleAuthenticatorTotp(string totp, byte[] key)
        {
            return _twoFactorAuthenticator.ValidateTwoFactorPIN(key, totp);
        }

        /// <summary>
        /// Generates google authenticator pin
        /// </summary>
        /// <param name="key">The encoded key</param>
        /// <param name="expiryDate">Expiry date</param>
        /// <returns>Pin</returns>
        public string GenerateGoogleAuthenticatorPin(byte[] key, DateTime expiryDate)
        {
            return _twoFactorAuthenticator.GetCurrentPIN(key, expiryDate);
        }

        /// <summary>
        /// Generate google authenticator pins
        /// </summary>
        /// <param name="key">the encoded key</param>
        /// <param name="expiryDate">Expiry date</param>
        /// <returns>A list of pins</returns>
        public string[] GenerateGoogleAuthenticatorPins(byte[] key, DateTime expiryDate)
        {
            return _twoFactorAuthenticator.GetCurrentPINs(key,expiryDate.TimeOfDay);
        }

        private readonly TwoFactorAuthenticator _twoFactorAuthenticator;
    }
}
