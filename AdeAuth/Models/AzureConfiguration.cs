using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages azure single sign on configuration
    /// </summary>
    public class AzureConfiguration
    {
        /// <summary>
        /// Audience of the token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Instance of the token
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Tenant id of azure subscription
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Type of azure application
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Name of the name claim type
        /// </summary>
        public string NameClaimType { get; set; }

        /// <summary>
        /// Authentication scheme
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}
