namespace AdeAuth.Models
{
    /// <summary>
    /// Manages users
    /// </summary>
    public interface IApplicationUser 
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// First name of user
        /// </summary>

        string FirstName { get; set; }

        /// <summary>
        /// Last name of user
        /// </summary>
        string LastName { get; set; }


        /// <summary>
        /// Username of user
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Email 
        /// </summary>
        string Email { get; set; }


        /// <summary>
        /// Verifies email confirmation
        /// </summary>
        bool EmailConfirmed { get; set; }


        /// <summary>
        /// Secret to encode/decode password
        /// </summary>
        string Salt {  get; set; }
         
        /// <summary>
        /// Encoded password 
        /// </summary>
        string PasswordHash { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// Verifies phone number confirmation
        /// </summary>
        bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Verifies if two factor authentication is enabled
        /// </summary>
        bool TwoFactorEnabled { get; set; }


        /// <summary>
        /// The two factor authentication type
        /// </summary>
        int TwoFactorType { get; set; }

        /// <summary>
        /// Authenticator key or url for qr code
        /// </summary>
        string AuthenticatorKey { get; set; }


        /// <summary>
        /// Verifies if the user is locked out
        /// </summary>
        bool LockoutEnabled { get; set; }

        IList<IApplicationRole> Roles { get; set; }
    }
}
