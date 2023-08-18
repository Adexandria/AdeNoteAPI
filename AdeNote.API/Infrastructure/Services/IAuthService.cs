using AdeNote.Models.DTOs;
using TasksLibrary.Models;
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
        ActionResult<string> GenerateMFAToken(Guid userId,string email,string refreshToken);
        ActionResult<DetailsDTO> ReadDetailsFromToken(string token);
        Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken);
        Task<ActionResult> IsTokenRevoked(string refreshToken);
    }
}
