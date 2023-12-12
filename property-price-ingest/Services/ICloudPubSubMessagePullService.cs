namespace property_price_ingest.Services
{
	public interface ICloudPubSubMessagePullService
	{
        Task PullMessagesAsync(CancellationToken stoppingToken);
    }
}

