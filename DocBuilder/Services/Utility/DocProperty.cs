namespace DocBuilder.Services.Utility
{
    internal class DocProperty
    {
        public DocProperty(string name, object attributeName)
        {
            PropertyName = name;
            AttributeName = attributeName;
        }
        public string PropertyName { get; set; }
        public object AttributeName { get; set; }
    }
}
