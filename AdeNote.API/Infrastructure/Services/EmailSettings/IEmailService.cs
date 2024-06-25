using AdeNote.Infrastructure.Utilities.EmailSettings;

namespace AdeNote.Infrastructure.Services.EmailSettings
{
    public interface IEmailService
    {
        public void SendMessage<T>(T email) where T : Email;
    }
}
