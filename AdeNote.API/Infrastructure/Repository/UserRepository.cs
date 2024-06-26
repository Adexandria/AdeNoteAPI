﻿using AdeAuth.Services;
using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(NoteDbContext dbContext,
            IPasswordManager passwordManager,
            ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
        {
            PasswordManager = passwordManager;
        }

        public async Task<bool> Add(User entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.Users.AddAsync(entity);

            var result = await SaveChanges();

            logger.LogInformation("Add user to database:{result}", result);

            return result;
        }

        public async Task<User> AuthenticateUser(string email, string password, AuthType authType)
        {
            var user = await Db.Users.
                   AsNoTracking()
                   .Include(s=>s.RefreshToken)
                   .Include(s => s.RecoveryCode)
                   .Where(s => s.Email == email)
                   .FirstOrDefaultAsync();

            if(authType != user.AuthenticationType)
            {
                return default;
            }

            if (user != null && user.AuthenticationType == AuthType.local)
            {
                var isVerified = PasswordManager.VerifyPassword(password, user.PasswordHash, user.Salt);
                if (isVerified)
                {
                    return user;
                }
            }

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await Db.Users
                .Include(s=>s.RefreshToken).Include(s=>s.RecoveryCode).Where(s => s.Email == email).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await Db.Users.Include(s => s.RecoveryCode)
              .Include(s=>s.RefreshToken)
              .FirstOrDefaultAsync(x => x.Id == userId);
        }


        public async Task<bool> IsPhoneNumberVerified(Guid userId)
        {
            var isPhoneNumberVerified = await Db.Users.
                  AsNoTracking().Where(s => s.Id == userId)
                  .Select(s => s.PhoneNumberConfirmed).FirstOrDefaultAsync();

            return isPhoneNumberVerified;
        }

        public async Task<bool> Remove(User entity)
        {
            Db.Users.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove user to database:{result}", result);

            return result;
        }

        public async Task<bool> Update(User entity)
        {
            Db.Users.Update(entity);

            var result = await SaveChanges();

            logger.LogInformation("Update user to database:{result}", result);

            return result;
        }


        public bool IsExist(string email)
        {
            return Db.Users.Any(s => s.Email == email);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

        public int GetNumberOfUsers()
        {
            var noOfUsers = Db.Users.Where(s => s.Role == Role.User).Count();

            return noOfUsers;
        }

        public readonly IPasswordManager PasswordManager; 
    }
}
