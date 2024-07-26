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
    public class ApplicationRefreshToken
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Expiry date 
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Verifies if the token is revoked
        /// </summary>

        public bool IsRevoked { get; set; }
    }
}
