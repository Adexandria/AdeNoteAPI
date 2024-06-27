namespace AdeNote.Models.DTOs
{

    /// <summary>
    /// Displays the details of the user
    /// </summary>
    public class UsersDTO
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="firstname">Name of the user</param>
        /// <param name="codes">Recovery codes</param>
        /// <param name="lastName">Last name of the user</param>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        public UsersDTO(Guid userId,string firstname, string lastName, string email,string? codes)
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
        
        /// <summary>
        /// Recovery codes
        /// </summary>
        public string[]? Codes { get; set; }


    }
}
