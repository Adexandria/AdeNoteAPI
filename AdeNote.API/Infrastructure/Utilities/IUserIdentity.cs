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

        /// <summary>
        /// Stores the email of the authenticated user
        /// </summary>
        public string Email { get; set; }
    }
}
