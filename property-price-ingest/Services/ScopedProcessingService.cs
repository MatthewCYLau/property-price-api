namespace property_price_ingest.Services
{
	public class ScopedProcessingService : IScopedProcessingService
    {
        private int _executionCount;
        private readonly ILogger<ScopedProcessingService> _logger;

        public ScopedProcessingService(
            ILogger<ScopedProcessingService> logger) =>
            _logger = logger;

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ++_executionCount;

                _logger.LogInformation(
                    "{ServiceName} working, execution count: {Count}",
                    nameof(ScopedProcessingService),
                    _executionCount);

                await Task.Delay(5_000, stoppingToken);
            }
        }
    }
}

