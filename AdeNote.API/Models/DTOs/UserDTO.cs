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
        public UserDTO(Guid userId,string firstname, string lastName, string email,string? codes)
        {
            UserId = userId;
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Codes = codes?.Split(',');
        }



        public Guid UserId { get; set; }
        /// <summary>
        /// Name of the user
        /// </summary>
        public string FirstName { get; set; }    

        public string LastName { get; set; }    
        /// <summary>
        ///  Email of the user
        /// </summary>
        public string Email { get; set; }  
        
        public string[]? Codes { get; set; }
    }
}
