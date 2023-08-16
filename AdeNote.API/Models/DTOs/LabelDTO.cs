namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO To display the label
    /// </summary>
    public class LabelDTO
    {
        /// <summary>
        /// Id of the label
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the label
        /// </summary>
        public string Label { get; set; }
    }
}