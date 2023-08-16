using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    public class UserToken : BaseClass
    {
        public UserToken()
        {

        }
        public UserToken(MFAType type, Guid userId)
        {
            IsMFAEnabled = true;
            AuthenticationType = type;
            UserId = userId;
        }

        public UserToken SetAuthenticatorKey(string key)
        {
            AuthenticatorKey = key;
            return this;
        }


        public bool IsMFAEnabled { get; set; }

        public MFAType AuthenticationType { get; set; }  

        public string AuthenticatorKey { get; set; }

        public User User { get; set; }


        [ForeignKey("User_id")]
        public Guid UserId { get; set; }
    }
}
