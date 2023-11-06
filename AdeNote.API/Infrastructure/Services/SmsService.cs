using AdeNote.Infrastructure.Utilities;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AdeNote.Infrastructure.Services
{ 
    /// <summary>
    ///  A sms service used to send message via sms
    /// </summary>
    public class SmsService : ISmsService
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="config">Reads the key/value pair from appsettings</param>
        /// <param name="loggerFactory">A factory used to create logs</param>
        public SmsService(IConfiguration config,ILoggerFactory loggerFactory)
        {
            _smsConfig = config.GetSection("TwilioConfiguration").Get<SmsConfiguration>() ?? 
                new SmsConfiguration(config.GetValue<string>("AdeTAccountKey"), 
                config.GetValue<string>("AdeAccountSecret"),
                config.GetValue<string>("AdePhonenumber"));
            _logger = loggerFactory.CreateLogger(typeof(SmsService));
        }

        /// <summary>
        /// Sends sms
        /// </summary>
        /// <param name="sms">An object that includes the phonenumber and message</param>
        public void SendSms(Sms sms)
        {
            new Thread(async () =>
            {
                TwilioClient.Init(_smsConfig.AccountKey, _smsConfig.AccountSecret);
                var message = await MessageResource.CreateAsync(body: sms.Message,
                    from: new PhoneNumber(_smsConfig.PhoneNumber),
                    to: new PhoneNumber(sms.PhoneNumber));
                _logger.LogInformation($"Sms sent status: {message.Status} at {message.DateSent}" +
                    $" Error: {message.ErrorMessage}");
            }).Start();
        }
        private readonly SmsConfiguration _smsConfig;
        private readonly ILogger _logger;
    }
}
