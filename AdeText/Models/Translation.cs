using System.Text.Json.Serialization;


namespace AdeText.Models
{
    public class Translation
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
