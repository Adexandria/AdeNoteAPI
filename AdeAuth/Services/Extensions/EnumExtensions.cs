using System.ComponentModel;

namespace AdeAuth.Services.Extensions
{
    /// <summary>
    /// Manages enum extensions
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get description in an enum
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum source)
        {
            var enumField = source.GetType().GetField(source.ToString());
            if (enumField == null)
                return string.Empty;

            var descriptionAttributes = (DescriptionAttribute[])enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttributes.Length > 0)
                return descriptionAttributes[0].Description;

            return source.ToString();
        }
    }
}
