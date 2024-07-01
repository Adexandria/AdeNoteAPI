namespace AdeNote.Infrastructure.Utilities.UserConfiguation
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