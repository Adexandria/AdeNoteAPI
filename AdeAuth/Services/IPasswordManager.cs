namespace AdeAuth.Services
{
    public interface IPasswordManager
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string currentPassword);
    }
}
