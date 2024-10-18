using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ChattyPie.Models.DTOs
{
    public interface ISubThreadDtos
    {
        [JsonProperty("subUserIds")]
        public List<string> SubUserIds { get; set; }

        [JsonProperty("threadId")]
        public string ThreadId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("userIds")]
        public List<string> UserIds { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public List<SubThreadDtos> SubThreads { get; set; }

    }
}