using AdeNote.Infrastructure.Utilities.ValidationAttributes;

namespace AdeNote.Models.DTOs
{
    public class ChangePasswordDTO
    {
        public string Password { get; set; }

        [Password]
        public string CurrentPassword { get; set; } 
    }
}
