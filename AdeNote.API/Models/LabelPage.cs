using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    /// <summary>
    /// A model to handle the many to many relationship
    /// </summary>
    public class LabelPage
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public LabelPage()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="pageId">A page id</param>
        /// <param name="labelId">A label id</param>
        public LabelPage(Guid pageId, Guid labelId)
        {
            PageId = pageId;
            LabelId = labelId;
        }
        /// <summary>
        /// Id of the label
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// Id of the page
        /// </summary>
        public Guid PageId { get; set; }
    }
}
