namespace AdeAuth.Models
{
    /// <summary>
    /// Manages user role
    /// </summary>
    public class ApplicationRole
    {
        /// <summary>
        /// Id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>

        public virtual string Name { get; set; }
    }
}
