namespace AdeNote.Models
{
    public class HangfireUser : BaseEntity
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
        public string Username { get; set; }    
        public string PasswordHash { get; set; }
        public string Salt {  get; set; }
    }
}
