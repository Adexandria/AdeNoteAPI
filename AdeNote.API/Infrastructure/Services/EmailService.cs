using AdeNote.Infrastructure.Utilities;
using RestSharp;
using RestSharp.Authenticators;

namespace AdeNote.Infrastructure.Services
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
        }
        public void SendMessage<T>(T email) where T : Email
        {
            new Thread(async () =>
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
                if(string.IsNullOrEmpty(email.PlainTextMessage))
                {
                    request.AddParameter("html", email.HtmlMessage);
                }
                var response = await client.ExecuteAsync(request);
                logger.LogInformation($"Message sent {response.StatusCode} at {DateTime.UtcNow} Errors: {response.ErrorMessage}");
            })
            {

            }.Start();
        }
        private EmailConfiguration emailConfig;
        private ILogger logger;
    }
}
