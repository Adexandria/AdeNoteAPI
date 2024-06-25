using AdeNote.Infrastructure.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace AdeNote.Infrastructure.Utilities.HealthChecks
{
    public class APIHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            int numberOfRetries = 0;

            HealthCheckResult result = HealthCheckResult.Degraded("Degraded result from API");

            while(numberOfRetries < 3)
            {
                try
                {
                    var responseTime = Stopwatch.StartNew();

                    context.Registration.Timeout = new TimeSpan(0, 0, 0, 0, 200);

                    if (responseTime.Elapsed <= context.Registration.Timeout)
                    {
                        result = HealthCheckResult.Healthy("Healthy result from API");
                        break;
                    }

                    throw new HealthCheckException("Unhealthy result from API");
                }
                catch (HealthCheckException ex)
                {
                    numberOfRetries++;
                    if (numberOfRetries == 3)
                    {
                        result = HealthCheckResult.Unhealthy(ex.Message);
                    }
                }
            }

            return Task.FromResult(result);
        }

    }
}
