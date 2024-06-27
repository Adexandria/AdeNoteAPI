using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class LoginDTO
    {

        [Required(ErrorMessage = "Enter email")]
        [EmailAddress]
        public string Email { get; set; }


        [Required(ErrorMessage ="Enter password")]
        public string Password { get; set; }
    }
}
