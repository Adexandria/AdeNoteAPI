using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    public class ValidDateTimeAttribute: ValidationAttribute
    {
        public ValidDateTimeAttribute(string errorMessage) :base(errorMessage) 
        {   
        }
        public override bool IsValid(object? value)
        {
            if(string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }

           return DateTime.TryParse(value.ToString(), out var dateTime);
        }
    }
}
