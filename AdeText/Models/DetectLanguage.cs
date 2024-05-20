using System.Text.Json.Serialization;


namespace AdeText.Models
{
    internal class DetectLanguage : IDetectLanguage
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
}
