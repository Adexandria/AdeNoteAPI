using DocBuilder.Services.Utility.Attributes;
using System.ComponentModel;


namespace DocBuilder.Services.Utility
{
    internal static class DocExtractor
    {
        const string errorMessage = "properties not found";
        public static string GetDescription<TAttribute>(this Enum value) where TAttribute : DescriptionAttribute
        {
            var propertyInfo = value.GetType().GetMember(value.ToString());
            if (propertyInfo.Length == 0)
            {
                throw new NullReferenceException(errorMessage);
            }
            if (propertyInfo[0].GetCustomAttributes(false).FirstOrDefault() is TAttribute attribute)
            {
                return attribute.Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string GetDescription(this Enum value)
        {
            var propertyInfo = value.GetType().GetMember(value.ToString());
            if (propertyInfo.Length == 0)
            {
                throw new NullReferenceException(errorMessage);
            }
            if (propertyInfo[0].GetCustomAttributes(false).FirstOrDefault() is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        ///  Get the property name and attribute name
        /// </summary>
        /// <typeparam name="TAttribute">Attribute field</typeparam>
        /// <param name="t">Type of the object to read</param>
        /// <returns>A dictionary</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static List<DocProperty> GetAttributeProperty<TAttribute>(Type t) where TAttribute : DocAttribute
        {
            var propertyInfo = t.GetProperties();
            var attributeType = typeof(TAttribute);
            if (propertyInfo.Length == 0)
                throw new NullReferenceException(errorMessage);

            var propertyNames = new List<DocProperty>();
            foreach (var info in propertyInfo)
            {
                if (info.GetCustomAttributes(attributeType, false).FirstOrDefault() is not DocAttribute attribute)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(attribute.FieldName))
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldName));
                else
                {
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldPosition));
                }
            }
            return propertyNames;
        }

        public static List<DocProperty> GetAttributeProperty<TAttribute, TEntity>()
            where TAttribute : DocAttribute
            where TEntity : class
        {
            var propertyInfo = typeof(TEntity).GetProperties();
            var attributeType = typeof(TAttribute);
            if (propertyInfo.Length == 0)
                throw new NullReferenceException(errorMessage);

            var propertyNames = new List<DocProperty>();
            foreach (var info in propertyInfo)
            {
                if (info.GetCustomAttributes(attributeType, false).FirstOrDefault() is not DocAttribute attribute)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(attribute.FieldName))
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldName));
                else
                {
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldPosition));
                }
            }
            return propertyNames;
        }

        public static List<DocProperty> GetAttributeProperty(Type attributeType, Type entityType)
        {
            var propertyInfo = entityType.GetProperties();
            if (propertyInfo.Length == 0)
                throw new NullReferenceException(errorMessage);

            var propertyNames = new List<DocProperty>();
            foreach (var info in propertyInfo)
            {
                if (info.GetCustomAttributes(attributeType, true).FirstOrDefault() is not DocAttribute attribute)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(attribute.FieldName))
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldName));
                else
                {
                    propertyNames.Add(new DocProperty(info.Name, attribute.FieldPosition));
                }
            }
            return propertyNames;
        }
    }
}
