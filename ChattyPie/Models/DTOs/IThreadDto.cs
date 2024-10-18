using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChattyPie.Models.DTOs
{
    public interface IThreadDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("user_ids")]
        public List<string> UserIds { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public List<SubThreadDtos> SubThreads { get; set; }
    }
}
