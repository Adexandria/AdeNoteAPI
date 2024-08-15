using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeCache.Services.Utilities
{
    /// <summary>
    /// Manages property information
    /// </summary>
    internal class PropertyInformation
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of the property
        /// </summary>
        public Type PropertyType { get; set; }
    }
}
