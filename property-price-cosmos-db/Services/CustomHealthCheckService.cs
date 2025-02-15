
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace property_price_cosmos_db.Services;

public class CustomHealthCheckService : IHealthCheck
{

    public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("Health check okay."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "Health check failed!"));
    }
}
