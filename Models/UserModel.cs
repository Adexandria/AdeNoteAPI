using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    public class UserModel : User
    {
        [ForeignKey("RefreshToken_id")]
        public Guid RefreshTokenId { get; set; }

        [ForeignKey("AccessToken_id")]
        public Guid AccessTokenId { get; set; }
    }
}
