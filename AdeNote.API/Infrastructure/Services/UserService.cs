using AdeAuth.Services;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;


namespace AdeNote.Infrastructure.Services
{
    public class UserService : IUserService
    {
        public UserService()
        {

        }

        public UserService(IUserRepository userRepository,IPasswordManager passwordManager)
        {
            _userRepository = userRepository;
            _passwordManager = passwordManager;
        }

        public async Task<ActionResult> UpdateUserPassword(Guid userId, string currentPassword,string password)
        {
            var currentUser = await _userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var isVerified = _passwordManager.VerifyPassword(currentPassword,currentUser.PasswordHash);

            if (!isVerified)
                return ActionResult.Failed("Failed to verify", StatusCodes.Status400BadRequest);

            var hashedPassword = _passwordManager.HashPassword(password);

            currentUser.PasswordHash = hashedPassword;

            var commitStatus = await _userRepository.Update(currentUser);

            if(!commitStatus)
               return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.Successful();
        }

        public async Task<ActionResult> ResetUserPassword(Guid userId,string password)
        {
            var currentUser = await _userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            var hashedPassword = _passwordManager.HashPassword(password);

            currentUser.PasswordHash = hashedPassword;

            var commitStatus = await _userRepository.Update(currentUser);

            if (!commitStatus)
                return ActionResult.Failed("Failed to update password", StatusCodes.Status400BadRequest);

            return ActionResult.Successful();

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

            return ActionResult.Successful();
        }

        public readonly IUserRepository _userRepository;
        public readonly IPasswordManager _passwordManager;
    }
}
