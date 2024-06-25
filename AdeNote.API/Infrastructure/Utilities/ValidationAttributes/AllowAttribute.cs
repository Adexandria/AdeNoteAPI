using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class AllowAttribute : ValidationAttribute
    {
        public AllowAttribute(string errorMessage, params string[] values) : base(errorMessage)
        {
            _values = values;
        }

        public override bool IsValid(object? value)
        {
            if(value == null)
            {
                return false;
            }

            return _values.Contains(value?.ToString());
        }

        private readonly string[] _values;
    }
}
