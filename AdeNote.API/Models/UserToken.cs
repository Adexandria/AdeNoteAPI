using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    /// <summary>
    /// A user token object
    /// </summary>
    public class UserToken : BaseClass
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public UserToken()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="type">Multifactor authentication type</param>
        /// <param name="userId"></param>
        public UserToken(MFAType type, Guid userId)
        {
            IsMFAEnabled = true;
            AuthenticationType = type;
            UserId = userId;
        }

        /// <summary>
        /// Set the authenticator key
        /// </summary>
        /// <param name="key">A key</param>
        /// <returns>A user token</returns>
        public UserToken SetAuthenticatorKey(string key)
        {
            AuthenticatorKey = key;
            return this;
        }

        /// <summary>
        /// Shows if multifactor authentication is enabled
        /// </summary>
        public bool IsMFAEnabled { get; set; }

        /// <summary>
        /// The authenication type
        /// </summary>
        public MFAType AuthenticationType { get; set; }  

        /// <summary>
        /// Authenication key
        /// </summary>
        public string AuthenticatorKey { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [ForeignKey("User_id")]
        public Guid UserId { get; set; }
    }
}
