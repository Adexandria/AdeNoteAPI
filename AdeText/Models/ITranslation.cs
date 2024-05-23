using System.Text.Json.Serialization;

namespace AdeText.Models
{
    public interface ITranslation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}