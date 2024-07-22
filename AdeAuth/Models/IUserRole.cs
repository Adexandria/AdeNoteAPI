using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    /// <summary>
    /// Manages user's role
    /// </summary>
    public interface IUserRole
    {
        /// <summary>
        /// Role id
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
