using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class ValidTicketIdAttribute : ValidationAttribute
    {
        public ValidTicketIdAttribute(string errorMessage): base(errorMessage)
        {
            
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Value cannot be empty or null");
            }

            var regex = new Regex("\\Atk[a-z0-9]{5}");

            return regex.IsMatch(value.ToString()) ? ValidationResult.Success : new ValidationResult("Invalid ticket Id");
        }
    }
}
