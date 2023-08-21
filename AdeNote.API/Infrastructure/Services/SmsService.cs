using AdeNote.Infrastructure.Utilities;
using Azure.Communication.Sms;

namespace AdeNote.Infrastructure.Services
{
    public class SmsService : ISmsService
    {
        public SmsService(IConfiguration config,ILogger log)
        {
            connectionString = config["CommunicationKey"];
            logger = log;
        }
        public void SendSms(Sms sms)
        {
            new Thread(async () =>
            {
                SmsClient smsClient = new(connectionString);
                var response = await smsClient.SendAsync("+23408129812808",sms.PhoneNumber,sms.Message);
                logger.LogInformation($"Sms sent Status code: {response.Value.Successful} at {DateTime.UtcNow} " +
                    $"Error Message:{response.Value.ErrorMessage}");
            }).Start();
        }
        private string connectionString;
        private ILogger logger;
    }
}
