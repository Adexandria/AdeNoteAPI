using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IEmailService
    {
        public void SendMessage<T>(T email) where T: Email;
    }
}
