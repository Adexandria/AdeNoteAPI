using System.Net;

namespace AdeText.Utilities
{
    internal class TranslationException : Exception
    {
        public TranslationException(string message) : base(message)
        {
                
        }

        public TranslationException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode; 
        }
        public HttpStatusCode StatusCode { get; set; }
    }
}
