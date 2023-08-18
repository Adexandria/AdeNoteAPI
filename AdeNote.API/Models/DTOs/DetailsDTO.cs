namespace AdeNote.Models.DTOs
{
    public class DetailsDTO
    {
        public DetailsDTO(string email, string userId, string refreshToken)
        {
            UserId = new Guid(userId);
            Email = email;
            RefreshToken = refreshToken;
        }
        public Guid UserId { get; set; }    
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
