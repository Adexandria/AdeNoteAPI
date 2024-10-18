using System.ComponentModel;

namespace AdeNote.Infrastructure.Utilities
{
    public enum MimeType
    {
        none,
        [Description("image/png")]
        png,
        [Description("text/html")]
        html,
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx,
        [Description("application/vnd.ms-excel")]
        xls,
        [Description("text/csv")]
        csv,
        [Description("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        docx,
        [Description("video/mp4")]
        mp4
    }
}
