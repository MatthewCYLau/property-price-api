namespace property_price_ingest.Services
{
	public interface IScopedProcessingService
	{
        Task GetDataAsync(CancellationToken stoppingToken, int maxExecutionCount);
    }
}

