using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdeText.Utilities
{
    internal class Root
    {
        [JsonPropertyName("translation")]
        public Language SupportedLanguage { get; set; }
    }
}
