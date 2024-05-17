using AdeNote.Infrastructure.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Enter first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter email")]
        [EmailAddress]
        public string Email { get; set; }

        [Password]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage = "Password does not match")]
        public string RetypePassword { get; set; }  
    }
}
