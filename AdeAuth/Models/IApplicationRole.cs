namespace AdeAuth.Models
{
    /// <summary>
    /// Manages user role
    /// </summary>
    public interface IApplicationRole
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Name of the role
        /// </summary>

        string Name { get; set; }
    }
}
