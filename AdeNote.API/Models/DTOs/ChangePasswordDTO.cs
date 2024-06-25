using AdeNote.Infrastructure.Utilities.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Invalid password")]
        public string Password { get; set; }

        [Password]
        public string CurrentPassword { get; set; } 
    }
}
