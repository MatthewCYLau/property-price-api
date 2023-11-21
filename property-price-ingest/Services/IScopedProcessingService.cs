namespace property_price_ingest.Services
{
	public interface IScopedProcessingService
	{
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}

