using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AdeNote.Infrastructure.Utilities.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public DatabaseHealthCheck(IConfiguration config)
        {
            connectionString = config.GetConnectionString("NotesDB");
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("SELECT 1", connection))
            {
                try
                {
                    connection.Open();
                    var result = await command.ExecuteScalarAsync(cancellationToken);

                    if (result != null && result.Equals(1))
                    {
                        return HealthCheckResult.Healthy();
                    }
                    else
                    {
                        return HealthCheckResult.Unhealthy("Database query did not return 1.");
                    }
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy(ex.Message);
                }
            }
        }

        private readonly string connectionString;
    }
}
