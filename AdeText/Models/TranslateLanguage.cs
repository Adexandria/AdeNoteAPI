
using System.Text.Json.Serialization;

namespace AdeText.Models
{
    internal class TranslateLanguage :ITranslateLanguage
    {
        [JsonPropertyName("translations")]
        public List<ITranslation> Translations { get; set; }
    }
}
