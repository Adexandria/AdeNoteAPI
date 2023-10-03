using System.ComponentModel;

namespace AdeNote.Infrastructure.Extension
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T source) where T : Enum
        {
            var enumField = typeof(T).GetField(source.ToString());
            if(enumField == null)
                return string.Empty;
           
            var descriptionAttributes = (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(descriptionAttributes.Length > 0)
                return descriptionAttributes[0].Description;
           
            return source.ToString();
        }

    }
}
