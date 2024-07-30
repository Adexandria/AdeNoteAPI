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
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Expiry date 
        /// </summary>
        public virtual DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Verifies if the token is revoked
        /// </summary>

        public virtual bool IsRevoked { get; set; }
    }
}
