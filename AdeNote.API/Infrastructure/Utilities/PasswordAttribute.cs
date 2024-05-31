using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AdeNote.Infrastructure.Utilities
{
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (true)
            {
               
                case bool _ when !Regex.IsMatch(value.ToString(), "[$!&*^]"):
                    return new ValidationResult("Password must have at least one special character eg $!&*^");
                case bool _ when !Regex.IsMatch(value.ToString(), "[A-Z]"):
                    return new ValidationResult("Password must have at least one capital letter");
                case bool _ when !Regex.IsMatch(value.ToString(), "[a-z]"):
                    return new ValidationResult("Password must have at least one small letter");
                case bool _ when !Regex.IsMatch(value.ToString(), "[0-9]"):
                    return new ValidationResult("Password must have at least one number");
                default:
                    return ValidationResult.Success;
            }

        }
    }
}
