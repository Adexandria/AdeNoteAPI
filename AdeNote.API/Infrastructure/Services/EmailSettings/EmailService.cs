using AdeNote.Infrastructure.Exceptions;
using AdeNote.Infrastructure.Utilities.EmailSettings;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace AdeNote.Infrastructure.Services.EmailSettings
{
    public class EmailService : IEmailService
    {
        public EmailService(IConfiguration config, ILoggerFactory loggerFactory)
        {
            emailConfig = config.GetSection("EmailConfiguration").Get<EmailConfiguration>() ?? new EmailConfiguration(
                    config.GetValue<string>("EmailConfiguration__Domain"),
                    config.GetValue<string>("EmailConfiguration__URL"),
                    config.GetValue<string>("EmailConfiguration__From"),
                    config.GetValue<string>("EmailConfiguration__APIKey"));
            logger = loggerFactory.CreateLogger(typeof(EmailService));
            retryConfiguration = 0;
        }
        public void SendMessage<T>(T email) where T : Email
        {
           ThreadPool.QueueUserWorkItem(o => Send(email));
        }


        public void SendMessages<T>(List<T> emails) where T : Email
        {
            Parallel.ForEach(emails, email => Send(email));
        }


        private void Send<T>(T email) where T : Email
        {
            try
            {
                var options = new RestClientOptions(emailConfig.URL)
                {
                    Authenticator = new HttpBasicAuthenticator("api", emailConfig.APIKey)
                };
                var client = new RestClient(options);
                RestRequest request = new();
                request.AddParameter("domain", emailConfig.Domain, ParameterType.UrlSegment);
                request.Resource = $"{emailConfig.Domain}/messages";
                request.AddParameter("from", $"AdeNote {emailConfig.From} ");
                request.AddParameter("to", email.To);
                request.AddParameter("subject", email.Subject);
                request.Method = Method.Post;
                if (string.IsNullOrEmpty(email.HtmlMessage))
                {
                    request.AddParameter("text", email.PlainTextMessage);
                }
                if (string.IsNullOrEmpty(email.PlainTextMessage))
                {
                    request.AddParameter("html", email.HtmlMessage);
                }
                var response = client.ExecuteAsync(request).Result;

                if(response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new MailGunException(response.StatusCode);
                }

                logger.LogInformation("Message sent {StatusCode} at {UtcNow} Errors: {ErrorMessage}", response.StatusCode, DateTime.UtcNow, response.ErrorMessage);
            }
            catch (MailGunException ex)
            {
                Task.Delay(TimeSpan.FromSeconds(120)).Wait();

                retryConfiguration++;

                if (retryConfiguration < retryConfiguration && ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Send(email);
                }
                return;
            }
            finally
            {
                retryConfiguration = 0;
            }
           
        }

        private EmailConfiguration emailConfig;
        private ILogger logger;
        private int retryConfiguration;
    }
}
