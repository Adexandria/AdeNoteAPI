using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
      AllowMultiple = false)]
    public class ValidGuidAttribute: ValidationAttribute
    {
        public ValidGuidAttribute(string errorMessage):base(errorMessage)
        {
                        
        }
        public override bool IsValid(object? value)
        {
          return (Guid)value != Guid.Empty;
        }
    }
}
