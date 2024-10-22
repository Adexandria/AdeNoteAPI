using Newtonsoft.Json;

namespace ChattyPie.Models
{
    public class BaseClass
    {
        public BaseClass()
        {
            Id = Guid.NewGuid().ToString();
        }
        [JsonProperty("id")]
        public string Id {  get; set; }
    }
}
