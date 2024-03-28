using AdeNote.Infrastructure.Repository;
using Autofac;
using TasksLibrary.Models;
using TasksLibrary.Models.Interfaces;
using TasksLibrary.Services;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }

        public UserService(IUser userRepository, IContainer container, IPasswordManager passwordManager)
        {
            _userRepository = userRepository;
            _userDb = container.Resolve<IUserRepository>();
            _passwordManager = passwordManager;
        }

        public async Task<ActionResult> UpdateUserPassword(Guid userId, string currentPassword,string password)
        {
            var currentUser = await _userDb.GetExistingEntityById(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var isVerified = _passwordManager.VerifyPassword(currentPassword,currentUser.PasswordHash, currentUser.Salt);

            if (!isVerified)
                return ActionResult.Failed("Failed to verify", StatusCodes.Status400BadRequest);

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            currentUser.PasswordHash = hashedPassword;
            currentUser.Salt = salt;

            var commitStatus = await _userRepository.UpdateUser(currentUser);

            if(!commitStatus)
               return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.Successful();
        }

        public async Task<ActionResult> ResetUserPassword(Guid userId,string password)
        {
            var currentUser = await _userDb.GetExistingEntityById(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var hashedPassword = _passwordManager.HashPassword(password, out string salt);

            currentUser.PasswordHash = hashedPassword;
            currentUser.Salt = salt;

            var commitStatus = await _userRepository.UpdateUser(currentUser);

            if (!commitStatus)
                return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.Successful();

        }

        public async Task<ActionResult<User>> GetUser(string email)
        {
            var currentUser = await _userDb.GetExistingUserByEmail(email);
            if (currentUser == null)
                return ActionResult<User>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            return ActionResult<User>.SuccessfulOperation(currentUser);
        }

        public async Task<ActionResult> IsUserExist(string email)
        {
            var isExist = await _userDb.IsExist(email);
            if (!isExist)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            return ActionResult.Successful();
        }

        public readonly IUser _userRepository;
        public readonly IUserRepository _userDb;
        public readonly IPasswordManager _passwordManager;
    }
}
