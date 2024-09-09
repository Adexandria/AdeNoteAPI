using System.ComponentModel;

namespace AdeNote.Models
{
    public enum Status
    {
        Pending,

        [Description("In-review")]
        Inreview,

        [Description("Resolved")]
        Resolved,

        Unresolved
    }
}