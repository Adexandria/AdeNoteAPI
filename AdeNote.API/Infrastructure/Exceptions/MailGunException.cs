using System.Net;

namespace AdeNote.Infrastructure.Exceptions
{
    public class MailGunException : Exception
    {
        public MailGunException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        public HttpStatusCode StatusCode { get; set; }
    }
}
