using Newtonsoft.Json;

namespace ChattyPie.Models
{
    public class SubThread : Thread
    {
        internal SubThread() { }
        public SubThread(string threadId, List<string> userIds,
            List<string> subUserIds, string message) : base(userIds,message)
        {
            ThreadId = threadId;
            SubUserIds = subUserIds;
        }

        public SubThread(string threadId, string userId, List<string> subUserIds, string message) : base(userId,message)
        {
            ThreadId = threadId;
            SubUserIds = subUserIds;
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

        public void AddSubUserIds(List<string> subUserIds)
        {
            SubUserIds.AddRange(subUserIds);
            Modified = DateTime.UtcNow;
        }

        public void AddSubUserId(string subUserId)
        {
            SubUserIds.Add(subUserId);
            Modified = DateTime.UtcNow;
        }

        public void RemoveSubUserIds(List<string> subUserIds)
        {
            subUserIds.ForEach(x => SubUserIds.Remove(x));
            Modified = DateTime.UtcNow;
        }

        public void RemoveSubUserId(string subUserId)
        {
            SubUserIds.Remove(subUserId);
            Modified = DateTime.UtcNow;
        }

        [JsonProperty("threadId")]
        public string ThreadId { get; set; }

        [JsonProperty("subUserIds")]
        public List<string> SubUserIds { get; set; }
    }
}
