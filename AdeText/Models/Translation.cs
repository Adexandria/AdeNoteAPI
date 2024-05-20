using System.Text.Json.Serialization;


namespace AdeText.Models
{
    internal class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
