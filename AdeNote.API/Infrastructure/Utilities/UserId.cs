namespace AdeNote.Infrastructure.Utilities
{
    public class UserId
    {
        public UserId(Guid id)
        {
             Id = id;
        }
        public Guid Id { get; set; }
    }
}