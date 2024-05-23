namespace AdeAuth.Models
{
    public interface IApplicationUser 
    {
        Guid Id { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string UserName { get; set; }

        string Email { get; set; }

        bool EmailConfirmed { get; set; }

        string Salt {  get; set; }
         
        string PasswordHash { get; set; }

        string PhoneNumber { get; set; }

        bool PhoneNumberConfirmed { get; set; }

        bool TwoFactorEnabled { get; set; }

        int TwoFactorType { get; set; }

        string AuthenticatorKey { get; set; }

        bool LockoutEnabled { get; set; }
    }
}
