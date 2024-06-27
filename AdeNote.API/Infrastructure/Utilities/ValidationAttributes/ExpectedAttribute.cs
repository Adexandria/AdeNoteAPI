using AdeNote.Infrastructure.Extension;
using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Utilities.ValidationAttributes
{
    public class ExpectedAttribute : ValidationAttribute
    {
        public ExpectedAttribute()
        {
            statusCode = StatusCodes.Status400BadRequest;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ActionResult actionResult = ActionResult.SuccessfulOperation();

            var objectType = value.GetType();

            switch(objectType)
            {
                case Type _ when objectType == typeof(int):
                    if((int) value <= 0)
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                break;

                case Type _ when objectType == typeof(DateTime):
                    if((DateTime) value == DateTime.MinValue)
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                break;
                case Type _ when objectType == typeof(Guid):
                    if((Guid) value == Guid.Empty)
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                break;
                case Type _ when objectType == typeof(string):
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                    break;
                case Type _ when objectType.BaseType == typeof(Array):
                    var array = value as Array;
                    if (array.Length == 0)
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                    break;
                default:
                    if(value == null)
                    {
                        actionResult = ActionResult.Failed(ErrorMessage, statusCode);
                    }
                    break;
            }

            return new ValidatorResult(actionResult);
        }

        private  readonly int statusCode;
    }
}
