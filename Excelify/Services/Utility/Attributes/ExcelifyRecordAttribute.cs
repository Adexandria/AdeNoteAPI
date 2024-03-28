
namespace Excelify.Services.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class ExcelifyRecordAttribute : Attribute
    {
        public string FieldName { get; set; }
        public int FieldPosition { get; set; }
    }
}
