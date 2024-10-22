using ChattyPie.Models;
using System.Text.Json.Serialization;
using Thread = ChattyPie.Models.Thread;

namespace AdeNote.Models
{
    public class TweetThread : Thread
    {
        public TweetThread(string userId, string message) : base(userId, message)
        {
        }

        public TweetThread(List<string> userIds, string message) : base(userIds, message)
        {
        }
    }
}
