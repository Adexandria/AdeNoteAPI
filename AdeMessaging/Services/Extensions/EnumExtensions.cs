using System.ComponentModel;

namespace AdeMessaging.Services.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum source) 
        {
            var enumField = source.GetType().GetField(source.ToString());
            if(enumField == null)
                return string.Empty;
           
            var descriptionAttributes = (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if(descriptionAttributes.Length > 0)
                return descriptionAttributes[0].Description;
           
            return source.ToString();
        }

    }
}
