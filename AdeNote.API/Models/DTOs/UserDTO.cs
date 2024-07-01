namespace AdeNote.Models.DTOs
{
    public class UserDTO : UsersDTO
    {
        public UserDTO(Guid userId, string firstname, string lastName, string email, string? codes,
           string refreshToken, string accessToken) : base(userId,firstname,lastName,email,codes)
        {
            UserId = userId;
            FirstName = firstname;
            LastName = lastName;
            Email = email;
            Codes = codes?.Split(',');
            RefreshToken = refreshToken;
            AccessToken = accessToken;
        }
        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }
    }
}
