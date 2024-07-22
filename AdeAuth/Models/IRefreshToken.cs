using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages refresh token
    /// </summary>
    public interface IRefreshToken
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        Guid UserId { get; set; }

        /// <summary>
        /// Expiry date 
        /// </summary>
        DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Verifies if the token is revoked
        /// </summary>

        bool IsRevoked { get; set; }
    }
}
