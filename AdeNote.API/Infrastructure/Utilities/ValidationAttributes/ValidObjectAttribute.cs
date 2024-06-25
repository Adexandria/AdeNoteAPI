using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    public class ValidObjectAttribute: ValidationAttribute
    {
        public ValidObjectAttribute(string errorMessage) : base(errorMessage) 
        {
            
        }

        public override bool IsValid(object? value)
        {
           return value != null;
        }
    }
}
