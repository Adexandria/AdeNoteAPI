using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface ISmsService
    {
        public void SendSms(Sms sms);
    }
}
