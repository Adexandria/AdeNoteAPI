using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface ILabelPageRepository
    {
        Task<bool> AddLabelToPage(Guid pageId, Guid labelId);
        Task<IList<LabelPage>> GetLabels(Guid pageId);
        Task<LabelPage> GetLabel(Guid pageId,Guid labelId);
        Task<bool> DeleteLabelFromPage(LabelPage currentPageLabel);
        Task<bool> DeleteLabelsFromPage(IList<LabelPage> pageLabels);
    }
}
