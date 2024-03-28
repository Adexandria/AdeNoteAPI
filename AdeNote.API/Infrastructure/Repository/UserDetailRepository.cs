using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class UserDetailRepository : Repository,IUserDetailRepository
    {
        public UserDetailRepository(NoteDbContext db) : base(db)
        {

        }
        /// <summary>
        /// Saves a new entity
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Add(UserDetail entity)
        {
           entity.Id = Guid.NewGuid();
           await Db.UserDetails.AddAsync(entity);
           return await SaveChanges<UserDetail>();
        }

        /// <summary>
        /// An interface that includes the details of a user
        /// </summary>
        public async Task<UserDetail> GetUserDetail(Guid userId)
        {
            return await Db.UserDetails.
                AsNoTracking().Include(s=>s.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        /// <summary>
        /// Checks if user's phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>A boolean value</returns>
        public async Task<bool> IsPhoneNumberVerified(Guid userId)
        {
            var isPhoneNumberVerified  = await Db.UserDetails.
                 AsNoTracking().Where(s=>s.UserId == userId)
                 .Select(s=>s.IsPhoneNumberVerified).FirstOrDefaultAsync();

            return isPhoneNumberVerified;
        }

        /// <summary>
        /// Remove an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Remove(UserDetail entity)
        {
            Db.UserDetails.Remove(entity);
            return await SaveChanges<UserDetail>();
        }

        /// <summary>
        /// Updates an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(UserDetail entity)
        {
            var currentUserDetail = Db.UserDetails.Where(s=>s.UserId == entity.UserId).FirstOrDefault();

            Db.Entry(currentUserDetail).CurrentValues.SetValues(entity);

            Db.Entry(currentUserDetail).State = EntityState.Modified;

            return await SaveChanges<UserDetail>();
        }
    }
}
