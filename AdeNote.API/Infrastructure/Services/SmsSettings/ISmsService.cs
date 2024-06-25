using AdeNote.Infrastructure.Utilities.SmsConfig;

namespace AdeNote.Infrastructure.Services.SmsSettings
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
