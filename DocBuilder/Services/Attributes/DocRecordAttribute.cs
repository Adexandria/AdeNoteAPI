namespace DocBuilder.Services.Utility.Attributes
{
    public class DocRecordAttribute : DocAttribute
    {
        public DocRecordAttribute(int fieldPosition)
        {
            if(fieldPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(fieldPosition),"field position must be greater than 0");
           
            FieldPosition = fieldPosition;
        }

        public DocRecordAttribute(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName), "Field name can not be empty or null");
           
            FieldName = fieldName;
        }
    }
}
