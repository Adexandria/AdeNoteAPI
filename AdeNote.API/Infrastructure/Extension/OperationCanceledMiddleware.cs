using Microsoft.AspNetCore.Http;

namespace AdeNote.Infrastructure.Extension
{
    public class OperationCanceledMiddleware : IMiddleware
    {
        public OperationCanceledMiddleware(RequestDelegate next, 
            ILogger<OperationCanceledMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation was cancelled");
                context.Response.StatusCode = 400;
                
            }
        }

        private readonly RequestDelegate _next;
        private readonly ILogger<OperationCanceledMiddleware> _logger;
    }
}
