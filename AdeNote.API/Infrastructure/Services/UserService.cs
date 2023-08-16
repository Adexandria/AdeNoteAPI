using AdeNote.Infrastructure.Repository;
using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ActionResult<UserDTO>> FetchUserById(Guid userId)
        {
            try
            {
                var currentUser = await _userRepository.GetUserbyId(userId);
                if (currentUser == null)
                    return ActionResult<UserDTO>.Failed("User does not exist", StatusCodes.Status400BadRequest);

                var userDTO = new UserDTO(currentUser.Name, currentUser.Email);

                return ActionResult<UserDTO>.SuccessfulOperation(userDTO);
            }
            catch (Exception ex)
            {
                return ActionResult<UserDTO>.Failed(ex.Message);
            }
           
        }
        public async Task<ActionResult<Guid>> GetUserByEmail(string email)
        {
            try
            {
                var currentUser = await _userRepository.GetUserByEmail(email);
                if (currentUser == null)
                    return ActionResult<Guid>.Failed("email or password not correct", StatusCodes.Status400BadRequest);

                return ActionResult<Guid>.SuccessfulOperation(currentUser.Id);
            }
            catch (Exception ex)
            {
                return ActionResult<Guid>.Failed(ex.Message);
            }
        }
    }
}
