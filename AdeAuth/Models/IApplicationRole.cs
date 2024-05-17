namespace AdeAuth.Models
{
    public interface IApplicationRole
    {
        Guid Id { get; set; }

        string Name { get; set; }

        Guid UserId { get; set; }
    }
}
