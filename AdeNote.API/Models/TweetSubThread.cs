using ChattyPie.Models;

namespace AdeNote.Models
{
    public class TweetSubThread : SubThread
    {
        public TweetSubThread(string threadId, List<string> userIds,
           List<string> subUserIds, string message) : base(threadId,userIds,subUserIds,message)
        {
        }

        public TweetSubThread(string threadId, string userId, List<string> subUserIds, string message) 
            : base(threadId,userId,subUserIds,message)
        {
           
        }
    }
}
