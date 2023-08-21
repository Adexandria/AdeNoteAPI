namespace AdeNote.Models.DTOs
{

    /// <summary>
    /// Displays the details of the user
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="name">Name of the user</param>
        /// <param name="email">Email of the user</param>
        public UserDTO(string name, string email)
        {
            Name = name;
            Email = email;
        }
        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }    
        
        /// <summary>
        ///  Email of the user
        /// </summary>
        public string Email { get; set; }   
    }
}
