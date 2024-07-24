using AdeAuth.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages multi-factor service
    /// </summary>
    public interface IMfaService
    {
        /// <summary>
        /// Sets up google authenticator 
        /// </summary>
        /// <param name="issuer">The organisation issuing the service</param>
        /// <param name="email">The email of the user</param>
        /// <param name="key">The encoded key</param>
        /// <returns></returns>
        IAuthenticator SetupGoogleAuthenticator(string issuer, string email, byte[] key);

        /// <summary>
        /// Verifies google authenticator one time password
        /// </summary>
        /// <param name="totp">Time-based one time password</param>
        /// <param name="key">The encoded key</param>
        /// <returns>boolean value</returns>
        bool VerifyGoogleAuthenticatorTotp(string totp, byte[] key);

        /// <summary>
        /// Generates google authenticator pin
        /// </summary>
        /// <param name="key">The encoded key</param>
        /// <param name="expiryDate">Expiry date</param>
        /// <returns>Pin</returns>
        string GenerateGoogleAuthenticatorPin(byte[] key, DateTime expiryDate);

        /// <summary>
        /// Generate google authenticator pins
        /// </summary>
        /// <param name="key">the encoded key</param>
        /// <param name="expiryDate">Expiry date</param>
        /// <returns>A list of pins</returns>
        string[] GenerateGoogleAuthenticatorPins(byte[] key, DateTime expiryDate);
    }
}
