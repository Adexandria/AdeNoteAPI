using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An Interface that includes the behaviour of the page labels object
    /// </summary>
    public interface ILabelPageRepository
    {
        /// <summary>
        /// Adds label to a particular page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <param name="labelId">a label id</param>
        /// <returns>A boolean value</returns>
        Task<bool> AddLabelToPage(Guid pageId, Guid labelId);

        /// <summary>
        /// Gets all labels in a page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <returns>a list of labels</returns>
        Task<IList<LabelPage>> GetLabels(Guid pageId);

        /// <summary>
        /// Gets a particular label of a page
        /// </summary>
        /// <param name="pageId">Page id</param>
        /// <param name="labelId">a label id</param>
        /// <returns>a label object</returns>
        Task<LabelPage> GetLabel(Guid pageId,Guid labelId);

        /// <summary>
        /// Deletes a particular label from a page
        /// </summary>
        /// <param name="currentPageLabel">A label page object</param>
        /// <returns>A boolean value</returns>
        Task<bool> DeleteLabelFromPage(LabelPage currentPageLabel);

        /// <summary>
        /// Deletes labels from a page
        /// </summary>
        /// <param name="pageLabels">A list of label page object</param>
        /// <returns>A boolean value</returns>
        Task<bool> DeleteLabelsFromPage(IList<LabelPage> pageLabels);
    }
}
