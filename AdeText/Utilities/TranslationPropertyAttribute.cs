namespace AdeText.Utilities
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    internal class TranslationPropertyAttribute : Attribute
    {
        public TranslationPropertyAttribute(string name, Type sourceType)
        {
            Name = name;
            SourceType = sourceType;
        }

        public string Name { get; }

        public Type SourceType { get; }
    }
}
