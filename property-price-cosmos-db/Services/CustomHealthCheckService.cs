
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public class CustomHealthCheckService(IOptions<CosmosDbOptions> options) : IHealthCheck
{

    private readonly CosmosDbOptions _options = options.Value;

    public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        CosmosClient cosmosClient = new(
            Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? _options.ConnectionString);

        if (cosmosClient != null)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("Health check okay."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "Health check failed!"));
    }
}
