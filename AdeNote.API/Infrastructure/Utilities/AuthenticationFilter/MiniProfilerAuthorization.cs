using System.Net.Http.Headers;
using System.Text;

namespace AdeNote.Infrastructure.Utilities.AuthenticationFilter
{
    public class MiniProfilerAuthorization
    {
        public MiniProfilerAuthorization(UserConfiguration _user)
        {
            user = _user;
        }
        public bool Authorize(HttpRequest request)
        {
            var httpContext = request.HttpContext;
            var values = httpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(values) || string.IsNullOrWhiteSpace(values))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(values);

            if (!_authenticationScheme.Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));

            if (string.IsNullOrEmpty(decodedToken) || string.IsNullOrWhiteSpace(decodedToken))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var userCredentials = decodedToken.Split(':');

            if (userCredentials.Length == 2
                && string.IsNullOrEmpty(userCredentials[0])
                && string.IsNullOrEmpty(userCredentials[1]))
            {
                SetChallengeResponse(httpContext);
                return false;
            }

            var currentUser = user.Username == userCredentials[0] && user.Password == userCredentials[1];

            if (currentUser)
            {
                httpContext.Response.StatusCode = 200;
                return true;
            }
            SetChallengeResponse(httpContext);
            return false;
        }

        private void SetChallengeResponse(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = 401;
            httpContext.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
            httpContext.Response.WriteAsync("Authentication is required.");
        }

        private readonly UserConfiguration user;
        private const string _authenticationScheme = "Basic";
    }
}

