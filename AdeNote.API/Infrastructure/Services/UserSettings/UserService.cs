using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Azure.Core;


namespace AdeNote.Infrastructure.Services.UserSettings
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }

        public UserService(IUserRepository userRepository, IPasswordManager passwordManager)
        {
            _userRepository = userRepository;
            _passwordManager = passwordManager;
        }


        public async Task<ActionResult> UpdateUserPassword(Guid userId, string currentPassword, string password)
        {
            var currentUser = await _userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var isVerified = _passwordManager.VerifyPassword(currentPassword, currentUser.PasswordHash, currentUser.Salt);

            if (!isVerified)
                return ActionResult.Failed("Failed to verify", StatusCodes.Status400BadRequest);

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            currentUser.SetPassword(hashedPassword, salt);

            currentUser.SetModifiedDate();

            var commitStatus = await _userRepository.Update(currentUser);

            if (!commitStatus)
                return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        public async Task<ActionResult> ResetUserPassword(Guid userId, string password)
        {
            var currentUser = await _userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            currentUser.SetPassword(hashedPassword, salt);

            var commitStatus = await _userRepository.Update(currentUser);

            currentUser.SetModifiedDate();

            if (!commitStatus)
                return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        public async Task<ActionResult<User>> GetUser(string email)
        {
            var currentUser = await _userRepository.GetUserByEmail(email);
            if (currentUser == null)
                return ActionResult<User>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            return ActionResult<User>.SuccessfulOperation(currentUser);

        }

        public async Task<ActionResult> IsUserExist(string email)
        {

            var isExist = _userRepository.IsExist(email);
            if (!isExist)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<UsersDTO>> GetUser(Guid userId)
        {
            var currentUser = await _userRepository.GetUser(userId);

            if (currentUser == null)
                return ActionResult<UsersDTO>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            return ActionResult<UsersDTO>.SuccessfulOperation(new UsersDTO(userId, currentUser.FirstName,
            currentUser.LastName, currentUser.Email, currentUser.RecoveryCode?.Codes));
        }

        public readonly IUserRepository _userRepository;
        public readonly IPasswordManager _passwordManager;
    }
}
