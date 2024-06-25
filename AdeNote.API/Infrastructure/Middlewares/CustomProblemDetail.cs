using Microsoft.AspNetCore.Http;

namespace AdeNote.Infrastructure.Middlewares
{
    public class CustomProblemDetail
    {

        public CustomProblemDetail(string errorMessage , int statusCode)
        {
            ErrorMessage = errorMessage;   
            StatusCode = statusCode;
        }
        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Status code
        /// </summary>
        public int StatusCode { get; }
    }
}