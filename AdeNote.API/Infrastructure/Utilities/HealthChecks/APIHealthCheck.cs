using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace AdeNote.Infrastructure.Utilities.HealthChecks
{
    public class APIHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var responseTime = Stopwatch.StartNew();

             context.Registration.Timeout = new TimeSpan(0,0,0,0,200);

            if (responseTime.Elapsed <= context.Registration.Timeout)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Healthy result from API"));
            }
            else if (responseTime.Elapsed > context.Registration.Timeout)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Degraded result from API"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from API"));
        }

    }
}
