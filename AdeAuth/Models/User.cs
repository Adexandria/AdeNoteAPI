using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    internal class User : ApplicationUser
    {
        public User():base()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public new Guid Id { get; set; }
        public new string FirstName { get; set; }
        public new string LastName { get; set; }
        public new string UserName { get; set; }
        public new string Email { get; set; }
        public new bool EmailConfirmed { get; set; }
        public new string Salt { get; set; }
        public new string PasswordHash { get; set ; }
        public new string PhoneNumber { get; set ; }
        public new bool PhoneNumberConfirmed { get; set; }
        public new bool TwoFactorEnabled { get; set; }
        public new int TwoFactorType { get; set; }
        public new string AuthenticatorKey { get; set; }
        public new bool LockoutEnabled { get; set; }
    }
}
