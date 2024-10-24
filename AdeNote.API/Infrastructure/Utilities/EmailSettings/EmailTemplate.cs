using System.ComponentModel;

namespace AdeNote.Infrastructure.Utilities.EmailSettings
{
    /// <summary>
    /// Manages the email template
    /// </summary>
    public enum EmailTemplate
    {
        /// <summary>
        /// Template for email logic notification
        /// </summary>
        [Description("LoginNotification")]
        LoginNotification,

        [Description("ResetTokenNotification")]
        ResetTokenNotification,

        [Description("MFAToken")]
        MfaRemovalTokenNotification,

        [Description("EmailConfirmation")]
        EmailConfirmationNotification,

        [Description("Passworrdless")]
        PasswordlessNotification,

        
        TicketUpdateNotification,

        SolvedTicketNotification,

        TweetNotification,

    }
}
