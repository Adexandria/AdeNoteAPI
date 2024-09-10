using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Extensions
{
    public static class EnumExtensions
    {
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
