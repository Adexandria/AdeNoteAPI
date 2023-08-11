using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles persisting and querying for page labels.
    /// </summary>
    public class LabelPageRepository : Repository, ILabelPageRepository
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        protected LabelPageRepository()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="db">Handles Transactions</param>
        public LabelPageRepository(NoteDbContext db) : base(db)
        {
           
        }

        /// <summary>
        /// Adds label to a particular page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <param name="labelId">a label id</param>
        /// <returns>A boolean value</returns>
        public async Task<bool> AddLabelToPage(Guid pageId, Guid labelId)
        {
            var pageLabel = new LabelPage(pageId, labelId);
            await Db.LabelPage.AddAsync(pageLabel);
            return await SaveChanges<LabelPage>();

        }

        /// <summary>
        /// Deletes a particular label from a page
        /// </summary>
        /// <param name="currentPageLabel">A label page object</param>
        /// <returns>A boolean value</returns>
        public async Task<bool> DeleteLabelFromPage(LabelPage currentPageLabel)
        {
             Db.LabelPage.Remove(currentPageLabel);
            return await SaveChanges<LabelPage>();
        }

        /// <summary>
        /// Deletes labels from a page
        /// </summary>
        /// <param name="pageLabels">A list of label page object</param>
        /// <returns>A boolean value</returns>
        public async Task<bool> DeleteLabelsFromPage(IList<LabelPage> pageLabels)
        {
            Db.LabelPage.RemoveRange(pageLabels);
            return await SaveChanges<LabelPage>();
        }

        /// <summary>
        /// Gets a particular label of a page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <param name="labelId">a label id</param>
        /// <returns>A page label</returns>
        public async Task<LabelPage> GetLabel(Guid pageId,Guid labelId)
        {
           return await Db.LabelPage
                .Where(s=>s.PageId == pageId && s.LabelId == labelId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all labels in a page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <returns>a list of page label</returns>
        public async Task<IList<LabelPage>> GetLabels(Guid pageId)
        {
            return await Db.LabelPage
                .Where(s=>s.PageId == pageId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
