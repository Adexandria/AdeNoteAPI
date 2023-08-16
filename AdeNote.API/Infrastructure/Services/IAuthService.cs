using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<ActionResult<string>> GetUserQrCode(Guid userId);
        Task<ActionResult> IsAuthenticatorEnabled(Guid userId);
        Task<ActionResult> IsAuthenticatorEnabled(string email);
        Task<ActionResult<AuthenticatorDTO>> SetEmailAuthenticator(Guid userId,string email);      
        ActionResult VerifyEmailAuthenticator(string email,string otp);
        ActionResult<string> GenerateMFAToken(string email);
        ActionResult<string> ReadEmailFromToken(string token);
    }
}
