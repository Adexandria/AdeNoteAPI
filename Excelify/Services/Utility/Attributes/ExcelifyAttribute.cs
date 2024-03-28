

namespace Excelify.Services.Utility.Attributes
{
    public class ExcelifyAttribute : ExcelifyRecordAttribute
    {
        public ExcelifyAttribute(int fieldPosition)
        {
            if(fieldPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(fieldPosition),"field position must be greater than 0");
           
            FieldPosition = fieldPosition;
        }

        public ExcelifyAttribute(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName), "Field name can not be empty or null");
           
            FieldName = fieldName;
        }
    }
}
