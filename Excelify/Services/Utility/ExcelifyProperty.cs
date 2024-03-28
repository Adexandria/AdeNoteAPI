

namespace Excelify.Services.Utility
{
    internal class ExcelifyProperty
    {
        public ExcelifyProperty(string name, object attributeName)
        {
            PropertyName = name;
            AttributeName = attributeName;
        }
        public string PropertyName { get; set; }
        public object AttributeName { get; set; } 
    }
}
