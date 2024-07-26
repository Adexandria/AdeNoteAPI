namespace AdeAuth.Models
{
    /// <summary>
    /// Manages users
    /// </summary>
    public class ApplicationUser 
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name of user
        /// </summary>

        public string FirstName { get; set; }

        /// <summary>
        /// Last name of user
        /// </summary>
        public string LastName { get; set; }


        /// <summary>
        /// Username of user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email 
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// Verifies email confirmation
        /// </summary>
        public bool EmailConfirmed { get; set; }


        /// <summary>
        /// Secret to encode/decode password
        /// </summary>
        public string Salt {  get; set; }
         
        /// <summary>
        /// Encoded password 
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Verifies phone number confirmation
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Verifies if two factor authentication is enabled
        /// </summary>
        public bool TwoFactorEnabled { get; set; }


        /// <summary>
        /// The two factor authentication type
        /// </summary>
        public int TwoFactorType { get; set; }

        /// <summary>
        /// Authenticator key or url for qr code
        /// </summary>
        public string AuthenticatorKey { get; set; }


        /// <summary>
        /// Verifies if the user is locked out
        /// </summary>
        public bool LockoutEnabled { get; set; }

    }
}
