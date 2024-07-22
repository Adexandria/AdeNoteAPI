using AdeAuth.Services;
using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;
using System.Text;


namespace AdeNote.Infrastructure.Utilities.AuthenticationFilter
{
    public class DashboardAuthenticationFilter : IDashboardAuthorizationFilter
    {

        public DashboardAuthenticationFilter(HangFireUserConfiguration _hangFireUser,ILoggerFactory loggerFactory)
        {
            hangFireUser = _hangFireUser;
            _logger = loggerFactory.CreateLogger<DashboardAuthenticationFilter>();
        }

        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var values = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(values) || string.IsNullOrWhiteSpace(values))
            {
                _logger.LogInformation("Authorization header is missing");
                SetChallengeResponse(httpContext);
                return false;
            }

            AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(values);

            if (!_authenticationScheme.Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation("Invalid authorization scheme");
                SetChallengeResponse(httpContext);
                return false;
            }

            var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));

            if (string.IsNullOrEmpty(decodedToken) || string.IsNullOrWhiteSpace(decodedToken))
            {
                _logger.LogInformation("Invalid username/password");
                SetChallengeResponse(httpContext);
                return false;
            }

            var userCredentials = decodedToken.Split(':');

            if (userCredentials.Length == 2
                && string.IsNullOrEmpty(userCredentials[0])
                && string.IsNullOrEmpty(userCredentials[1]))
            {
                _logger.LogInformation("Invalid username/password");
                SetChallengeResponse(httpContext);
                return false;
            }

            var currentUser = hangFireUser.Username == userCredentials[0] && hangFireUser.Password == userCredentials[1];

            if (currentUser)
            {
                _logger.LogInformation("User has successfully authenticated");
                httpContext.Response.StatusCode = 200;
                return true;
            }
            _logger.LogInformation("Invalid username/password");
            SetChallengeResponse(httpContext);
            return false;
        }

        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.WriteAsync("Authentication is required.");
        }

        private readonly HangFireUserConfiguration hangFireUser;
        private readonly ILogger _logger;
        private const string _authenticationScheme = "Basic";
    }
}
