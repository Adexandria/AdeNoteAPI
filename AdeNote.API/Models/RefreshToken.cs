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
        public override Guid Id { get; set; }
        public override string Token { get; set; }
        public override Guid UserId { get; set; }
        public  User User { get; set; }
        public override DateTime ExpiryDate { get; set; }
        public override bool IsRevoked { get; set; }
    }
}
