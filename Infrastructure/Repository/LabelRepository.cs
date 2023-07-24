using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class LabelRepository : Repository, ILabelRepository
    {
        public LabelRepository(NoteDbContext noteDb) : base(noteDb)
        {
        }

        public async Task<bool> Add(Label entity)
        {
            entity.Id = Guid.NewGuid();
            await _db.Labels.AddAsync(entity);
            return await SaveChanges<Label>();
        }

        public IQueryable<Label> GetAll()
        {
            return _db.Labels.OrderBy(s=>s.Id);
        }

        public async Task<Label> GetAsync(Guid id)
        {
            return await _db.Labels.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Label> GetByNameAsync(string name)
        {
            return await _db.Labels.AsNoTracking().FirstOrDefaultAsync(s => s.Title.Equals(name));
        }

        public async Task<bool> Remove(Label entity)
        {
            _db.Remove(entity);
            return await SaveChanges<Label>();
        }

        public async Task<bool> Update(Label entity)
        {
            var currentLabel = await  _db.Labels
                .FirstOrDefaultAsync(s => s.Id == entity.Id);

            _db.Entry(currentLabel).CurrentValues.SetValues(entity);

            return await SaveChanges<Label>();
        }
    }
}
