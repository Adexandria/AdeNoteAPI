namespace AdeNote.Infrastructure.Utilities.EmailSettings
{
    public class Email
    {
        public Email(string to, string subject)
        {
            To = to;
            Subject = subject;
        }

        public Email SetHtmlContent(string htmlContent)
        {
            HtmlMessage = htmlContent;
            return this;
        }

        public Email SetPlainContent(string plainTextContent)
        {
            PlainTextMessage = plainTextContent;
            return this;
        }
        public string PlainTextMessage { get; set; }
        public string HtmlMessage { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
    }
}
