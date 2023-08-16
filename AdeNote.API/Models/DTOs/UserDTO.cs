namespace AdeNote.Models.DTOs
{
    public class UserDTO
    {
        public UserDTO(string name, string email)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }    
        public string Email { get; set; }   
    }
}
