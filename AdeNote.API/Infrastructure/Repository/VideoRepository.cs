using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class VideoRepository : Repository<Video>,IVideoRepository
    {
        public VideoRepository(NoteDbContext noteDb, ILoggerFactory loggerFactory)
            : base(noteDb, loggerFactory)
        {

        }


        public async Task<bool> Add(Video entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.Videos.AddAsync(entity);

            var result = await SaveChanges();

            logger.LogInformation("Add video to database: {result}", result);

            return result;
        }

        public async Task<bool> Remove(Video entity)
        {
            Db.Videos.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Delete video from database: {result}", result);

            return result;
        }

        public async Task<bool> Update(Video entity)
        {
            var currentVideo = await Db.Videos
                .FirstOrDefaultAsync(s => s.Id == entity.Id && s.PageId == entity.PageId);

            Db.Entry(currentVideo).CurrentValues.SetValues(entity);

            Db.Entry(currentVideo).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update video in database: {result}", result);

            return result;
        }
    }
}
