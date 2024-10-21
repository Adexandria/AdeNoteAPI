using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages authentication configuration
    /// </summary>
    public class TokenConfiguration
    {
        /// <summary>
        ///  Audience of the token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Issuer of the token
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Secret to encode token
        /// </summary>
        public string TokenSecret { get; set; }

        /// <summary>
        /// Authentication scheme
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}
