using System.ComponentModel.DataAnnotations.Schema;

namespace AdeAuth.Models
{

    /// <summary>
    /// Manages users
    /// </summary>
    [NotMapped]
    public class ApplicationUser 
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// First name of user
        /// </summary>

        public virtual string FirstName { get; set; }

        /// <summary>
        /// Last name of user
        /// </summary>
        public virtual string LastName { get; set; }


        /// <summary>
        /// Username of user
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Email 
        /// </summary>
        public virtual string Email { get; set; }


        /// <summary>
        /// Verifies email confirmation
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }


        /// <summary>
        /// Secret to encode/decode password
        /// </summary>
        public virtual string Salt {  get; set; }
         
        /// <summary>
        /// Encoded password 
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Verifies phone number confirmation
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Verifies if two factor authentication is enabled
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }


        /// <summary>
        /// The two factor authentication type
        /// </summary>
        public virtual int TwoFactorType { get; set; }

        /// <summary>
        /// Authenticator key or url for qr code
        /// </summary>
        public virtual string AuthenticatorKey { get; set; }


        /// <summary>
        /// Verifies if the user is locked out
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }
    }
}
