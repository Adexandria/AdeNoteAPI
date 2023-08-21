namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// Displays the user details
    /// </summary>
    public class DetailsDTO
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        public DetailsDTO(string email, string userId, string refreshToken)
        {
            UserId = new Guid(userId);
            Email = email;
            RefreshToken = refreshToken;
        }
        /// <summary>
        /// User id 
        /// </summary>
        public Guid UserId { get; set; }    
        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Refresh token generated
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
