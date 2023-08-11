using AdeNote.Db;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Commits new and tracked changes in the objects
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        public Repository()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="noteDb">Handles Transactions</param>
        public Repository(NoteDbContext noteDb)
        {
            Db = noteDb;
        }

        /// <summary>
        /// A property to handle transactions
        /// </summary>
        public NoteDbContext Db { get; set; }

        /// <summary>
        /// Save changes. \n
        /// This will catch conflict exception 
        /// and update the entity with the database entity
        /// </summary>
        /// <typeparam name="T">A generic type T</typeparam>
        /// <returns>A boolean value</returns>
        /// <exception cref="NotSupportedException">Thrown if the type is not supported</exception>
        public virtual async Task<bool> SaveChanges<T>() where T : class
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await Db.SaveChangesAsync();
                    if (commitedResult == 0)
                    {
                        saved = false;
                        break;
                    }  
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is T)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var databaseValue = databaseValues[property];
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return saved;

        }
        
    }
}
