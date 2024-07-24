using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Excelify.Services.Exceptions
{
    /// <summary>
    /// Handles cache exception
    /// </summary>
    public class CacheException : Exception
    {

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="message">Error message</param>
        public CacheException(string message):base(message)
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public CacheException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
