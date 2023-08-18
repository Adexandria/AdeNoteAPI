namespace AdeNote.Infrastructure.Utilities
{
    /// <summary>
    /// Handles the user identity
    /// </summary>
    public interface IUserIdentity
    {
        /// <summary>
        /// Stores the user id of the authenticated user
        /// </summary>
        public Guid UserId { get; set; }

        public string Email { get; set; }
    }
}
