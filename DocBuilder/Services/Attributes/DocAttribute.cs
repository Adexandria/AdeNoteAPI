
namespace DocBuilder.Services.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class DocAttribute : Attribute
    {
        public string FieldName { get; set; }
        public int FieldPosition { get; set; }
    }
}
