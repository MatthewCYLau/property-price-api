namespace property_price_ingest.Services
{
	public interface IScopedProcessingService
	{
        Task GetPropertiesCount(CancellationToken stoppingToken, int maxExecutionCount);
    }
}

