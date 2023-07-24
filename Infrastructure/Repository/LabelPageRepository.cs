using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class LabelPageRepository : Repository, ILabelPageRepository
    {
        public LabelPageRepository(NoteDbContext db) : base(db)
        {
           
        }
        public async Task<bool> AddLabelToPage(Guid pageId, Guid labelId)
        {
            var pageLabel = new LabelPage(pageId, labelId);
            await _db.LabelPage.AddAsync(pageLabel);
            return await SaveChanges<LabelPage>();

        }

        public async Task<bool> DeleteLabelFromPage(LabelPage currentPageLabel)
        {
             _db.LabelPage.Remove(currentPageLabel);
            return await SaveChanges<LabelPage>();
        }

        public async Task<bool> DeleteLabelsFromPage(IList<LabelPage> pageLabels)
        {
            _db.LabelPage.RemoveRange(pageLabels);
            return await SaveChanges<LabelPage>();
        }

        public async Task<LabelPage> GetLabel(Guid pageId,Guid labelId)
        {
           return await _db.LabelPage
                .Where(s=>s.PageId == pageId && s.LabelId == labelId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IList<LabelPage>> GetLabels(Guid pageId)
        {
            return await _db.LabelPage
                .Where(s=>s.PageId == pageId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
