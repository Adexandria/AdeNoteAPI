﻿using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services
{
    public interface IUserService
    {
        Task<ActionResult> ResetUserPassword(Guid userId, string password);

        Task<ActionResult> UpdateUserPassword(Guid userId, string currentPassword, string password);

        Task<ActionResult<User>> GetUser(string email);

        ActionResult<StatisticsDto> GetStatistics();

        Task<ActionResult<UserDTO>> GetUser(Guid userId);

        Task<ActionResult> IsUserExist(string email);
    }
}
