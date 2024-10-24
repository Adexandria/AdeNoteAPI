namespace AdeNote.Models.DTOs
{
    public class NotifyUser
    {
        public NotifyUser(string email, string firstName, string lastName)
        {
            Email = email;
            Name = $"{firstName} {lastName}";
        }

        public string Email { get; set; }
        public string Name { get; set; }
    }
}
