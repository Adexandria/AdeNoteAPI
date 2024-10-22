using Newtonsoft.Json;


namespace ChattyPie.Models
{
    public class Thread : BaseClass
    {
        internal Thread()
        {
            
        }
        public Thread(string userId, string message)
        {
            UserIds = new List<string>()
            {
                userId
            };
            Message = message;
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }

        public Thread(List<string> userIds, string message)
        {
            UserIds = userIds;
            Message = message;
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }

        public void AddUserIds(List<string> userIds)
        {
            UserIds.AddRange(userIds);
            Modified = DateTime.UtcNow;
        }

        public void AddUserId(string userId)
        {
            UserIds.Add(userId);
            Modified = DateTime.UtcNow;
        }

        public void RemoveUserIds(string userId)
        {
            UserIds.Remove(userId);
            Modified = DateTime.UtcNow;
        }

        public void RemoveUserIds(List<string> userIds)
        {
            userIds.ForEach(x => UserIds.Remove(x));
            Modified = DateTime.UtcNow;
        }

        public void UpdateMessage(string message)
        {
            Message = message;
            Modified = DateTime.UtcNow;
        }

        [JsonProperty("userIds")]
        public List<string> UserIds { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
}
