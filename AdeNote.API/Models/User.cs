using AdeAuth.Models;
using System.ComponentModel.DataAnnotations;


namespace AdeNote.Models
{
    public class User : ApplicationUser, IBaseEntity
    {
        public User() { }

        public User(string firstName, string lastName, string email, AuthType authType)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AuthenticationType = authType;
            Role = Role.User;
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }

        public User(string firstName, string lastName, string email, AuthType authType, Role role)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AuthenticationType = authType;
            Role = role;
            Created = DateTime.UtcNow;
            Modified = DateTime.UtcNow;
        }
        public void EnableTwoFactor(MFAType twoFactorType, string authenticatorKey = null)
        {
            TwoFactorEnabled = true;
            TwoFactorType = (int)twoFactorType;
            AuthenticatorKey = authenticatorKey;
        }

        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            TwoFactorType = 0;
            AuthenticatorKey = null;
        }

        public void SetPassword(string hashPassword, string salt)
        {
            PasswordHash = hashPassword;
            Salt = salt;
        }

        public void ConfirmEmailVerification()
        {
            EmailConfirmed = true;
        }

        public void ConfirmPhoneNumberVerification()
        {
            PhoneNumberConfirmed = true;
        }

        public void EnableLockOut()
        {
            LockoutEnabled = true;
        }

        public void SetModifiedDate()
        {
            Modified = DateTime.UtcNow;
        }


        public void CreateRecoveryCodes()
        {
            RecoveryCode = new RecoveryCode(Id);
        }

        [Key]
        public override Guid Id { get; set; }
        public override string FirstName { get; set; }
        public override string LastName { get; set; }
        public override string? UserName { get; set; }
        public override string Email { get; set; }
        public override bool EmailConfirmed { get; set; }
        public override string? PasswordHash { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public override string? PhoneNumber { get; set; }
        public override bool PhoneNumberConfirmed { get; set; }
        public override bool TwoFactorEnabled { get; set; }
        public override int TwoFactorType { get; set; }
        public override string? AuthenticatorKey { get; set; }
        public override bool LockoutEnabled { get; set; }
        public AuthType AuthenticationType { get; set; }
        public IList<Book> Books { get; set; } = new List<Book>();
        public override string? Salt { get; set; }
        public RecoveryCode RecoveryCode { get; set; }
        public Role Role { get; set; }
        public IList<Ticket> Tickets { get; set; } = new List<Ticket>();
        public  DateTime Created { get; set; }
        public  DateTime Modified { get; set; }
    }
}
