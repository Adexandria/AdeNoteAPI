using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
      AllowMultiple = false)]
    public class ValidIntAttribute: ValidationAttribute
    {
        public ValidIntAttribute(string errorMessage, int minValue = 0): base(errorMessage) 
        {
            _minValue = minValue;
        }

        public ValidIntAttribute(string errorMessage, int minValue , int maxValue) : base(errorMessage)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }


        public override bool IsValid(object? value)
        {
           return (int) value >= _minValue && (int)value <= _maxValue;
        }

        private readonly int _minValue= int.MinValue;
        private readonly int _maxValue = int.MaxValue;
    }
}
