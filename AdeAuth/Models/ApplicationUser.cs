using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    internal class ApplicationUser : BaseEntity, IApplicationUser
    {
        public ApplicationUser():base()
        {   }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set ; }
        public string PhoneNumber { get; set ; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int TwoFactorType { get; set; }
        public string AuthenticatorKey { get; set; }
        public bool LockoutEnabled { get; set; }

        public IList<IApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
    }
}
