using System.ComponentModel;

namespace AdeNote.Infrastructure.Utilities
{
    public enum MimeType
    {
        [Description("image/png")]
        png,
        [Description("text/html")]
        html,
        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        xlsx,
        [Description("application/vnd.ms-excel")]
        xls,
        [Description("text/csv")]
        csv
    }
}
