using System.Text.Json.Serialization;

namespace AdeText.Models
{
    public class SupportedLanguage : ISupportedLanguage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
