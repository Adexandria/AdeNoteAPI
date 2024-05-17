using AdeAuth.Models;


namespace AdeNote.Models
{
    public class RefreshToken : BaseClass, IRefreshToken
    {
        public RefreshToken()
        {
                
        }
        public RefreshToken(string refreshToken, Guid userId, DateTime expiryDate)
        {
            Token = refreshToken;
            UserId = userId;
            ExpiryDate = expiryDate;
        }


        public void RevokeToken()
        {
            IsRevoked = true;
        }

        public string Token { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
