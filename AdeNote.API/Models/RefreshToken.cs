using AdeAuth.Models;
using SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;
using System.ComponentModel.DataAnnotations;


namespace AdeNote.Models
{
    public class RefreshToken : IBaseEntity
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
        public  Guid Id { get; set; }
        public  string Token { get; set; }
        public  Guid UserId { get; set; }
        public  User User { get; set; }
        public  DateTime ExpiryDate { get; set; }
        public  bool IsRevoked { get; set; }
    }
}
