namespace AdeNote.Models
{
    /// <summary>
    /// Multifactor authentication type accepted
    /// </summary>
    public enum MFAType
    {
        none,
        /// <summary>
        /// Sms
        /// </summary>
        sms,
        /// <summary>
        /// Authentication application like google
        /// </summary>
        google
    }
}