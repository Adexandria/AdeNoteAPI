using AdeNote.Infrastructure.Utilities.EmailSettings;

namespace AdeNote.Infrastructure.Services.EmailSettings
{
    public interface IEmailService
    {
        public void SendMessage<T>(T email) where T : Email;

        public void SendMessages<T>(List<T> emails) where T : Email;
    }
}
