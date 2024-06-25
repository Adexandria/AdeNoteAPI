using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    public class ValidCollectionAttribute: ValidationAttribute
    {
        public ValidCollectionAttribute(string errorMessage, int minLength = 0) : base(errorMessage)
        {
            _minLength = minLength;     
        }

        public override bool IsValid(object? value)
        {
            var arrayValue = value as Array;

            return arrayValue.Length > _minLength;
        }

        private readonly int _minLength;
    }
}
