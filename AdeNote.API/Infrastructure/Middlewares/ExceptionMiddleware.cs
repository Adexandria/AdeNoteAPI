using Excelify.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace AdeNote.Infrastructure.Middlewares
{
    public class ExceptionMiddleware
    {
        public ExceptionMiddleware(RequestDelegate requestDelegate, ILoggerFactory loggerFactory)
        {
            _requestDelegate = requestDelegate;       
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }
        /// <summary>
        /// Handles http request
        /// </summary>
        private readonly RequestDelegate _requestDelegate;

        /// <summary>
        /// Triggers when a request has been made and handles exceptions if any
        /// </summary>
        /// <param name="httpContext">Includes information about http request</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _requestDelegate(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Handles the error messages being displayed
        /// </summary>
        /// <param name="context">Includes information about http request</param>
        /// <param name="exception">Exception thrown</param>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var error = GetError(exception);

            var responseObject = new ObjectResult(error.ErrorMessage)
            {
                StatusCode = error.StatusCode,

            };
            var result = JsonConvert.SerializeObject(responseObject);

            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = error.StatusCode;
            _logger.LogError("Failed Operation, StatusCode: {StatusCode}, Error:{Message}", error.StatusCode, error.ErrorMessage);
            return context.Response.WriteAsync(result);
        }

        /// <summary>
        /// Get error details
        /// </summary>
        /// <param name="exception">Exception thrown</param>
        /// <returns></returns>
        private CustomProblemDetail GetError(Exception exception)
        {
            if (exception is ValidationException v)
            {
                return new CustomProblemDetail(v.ValidationResult.ErrorMessage, StatusCodes.Status400BadRequest);
            }
            else if (exception is AuthenticationException e)
            {
                return new CustomProblemDetail(e.Message, StatusCodes.Status401Unauthorized);
            } else if (exception is OperationCanceledException o)
            {
                return new CustomProblemDetail("Operation has been cancelled", StatusCodes.Status400BadRequest);
            } else if (exception is CacheException cacheExpection)
            {
                return new CustomProblemDetail(cacheExpection.Message, StatusCodes.Status500InternalServerError);
            }
            else
            {
                return new CustomProblemDetail(exception.Message, StatusCodes.Status500InternalServerError);
            }
        }

        private ILogger<ExceptionMiddleware> _logger {  get; set; }
    }
}

