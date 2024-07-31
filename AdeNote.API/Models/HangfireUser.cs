using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    public class HangfireUser : IBaseEntity
    {
        public HangfireUser()
        {
            
        }
        public HangfireUser(string userName)
        {
            Username = userName;
        }

        public void SetPassword(string password,string salt)
        {
            PasswordHash = password;
            Salt = salt;
        }

        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }    
        public string PasswordHash { get; set; }
        public string Salt {  get; set; }
    }
}
