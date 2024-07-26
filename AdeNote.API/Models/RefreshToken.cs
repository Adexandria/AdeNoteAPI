using AdeAuth.Models;
using SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;
using System.ComponentModel.DataAnnotations;


namespace AdeNote.Models
{
    public class RefreshToken : ApplicationRefreshToken, IBaseEntity
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

        [Key]
        public new Guid Id { get; set; }
        public new string Token { get; set; }
        public new Guid UserId { get; set; }
        public new User User { get; set; }
        public new DateTime ExpiryDate { get; set; }
        public new bool IsRevoked { get; set; }
    }
}
