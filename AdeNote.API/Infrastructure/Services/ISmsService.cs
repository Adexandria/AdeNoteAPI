using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    ///  A sms service used to send message via sms
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends sms
        /// </summary>
        /// <param name="sms">An object that includes the phonenumber and message</param>
        public void SendSms(Sms sms);
    }
}
