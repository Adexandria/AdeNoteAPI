namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to display a page
    /// </summary>
    /// <inheritdoc/>
    public class PageDTO : BookPages
    {
        /// <summary>
        /// A content of a page
        /// </summary>
        public string Content { get; set; }

        public IList<VideoDTO> Videos { get; set; }

        /// <summary>
        /// A list of labels
        /// </summary>
        public IList<LabelDTO> Labels { get; set; }
    }
}
