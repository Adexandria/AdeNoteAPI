using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles persisting and querying for Label objects
    /// </summary>
    public class LabelRepository : Repository, ILabelRepository
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        protected LabelRepository()
        {

        }
        
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="noteDb">Handles transaction</param>
        public LabelRepository(NoteDbContext noteDb) : base(noteDb)
        {
        }
        /// <summary>
        /// Saves a new label
        /// </summary>
        /// <param name="entity">A label object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Add(Label entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.Labels.AddAsync(entity);
            return await SaveChanges<Label>();
        }

        /// <summary>
        /// Get all labels
        /// </summary>
        /// <returns>A list of labels</returns>
        public IQueryable<Label> GetAll()
        {
            return Db.Labels.OrderBy(s=>s.Id);
        }

        /// <summary>
        /// Get a particular label
        /// </summary>
        /// <param name="id">label id</param>
        /// <returns>boolean value</returns>
        public async Task<Label> GetAsync(Guid id)
        {
            return await Db.Labels.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }
        /// <summary>
        /// Gets a label by name
        /// </summary>
        /// <param name="name">label's name</param>
        /// <returns>Label</returns>
        public async Task<Label> GetByNameAsync(string name)
        {
            return await Db.Labels.AsNoTracking().FirstOrDefaultAsync(s => s.Title.Equals(name));
        }

        /// <summary>
        /// Remove an existing entity
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Remove(Label entity)
        {
            Db.Remove(entity);
            return await SaveChanges<Label>();
        }

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(Label entity)
        {
            var currentLabel = await  Db.Labels
                .FirstOrDefaultAsync(s => s.Id == entity.Id);

            Db.Entry(currentLabel).CurrentValues.SetValues(entity);

            Db.Entry(currentLabel).State = EntityState.Modified;

            return await SaveChanges<Label>();
        }
    }
}
