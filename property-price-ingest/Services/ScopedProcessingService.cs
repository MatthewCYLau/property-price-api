using MongoDB.Driver;
using property_price_api.Data;

namespace property_price_ingest.Services
{
	public class ScopedProcessingService : IScopedProcessingService
    {
        private int _executionCount = 1;
        private readonly ILogger<ScopedProcessingService> _logger;
        private readonly MongoDbContext _context;

        public ScopedProcessingService(
            ILogger<ScopedProcessingService> logger,
            MongoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task GetPropertiesCount(CancellationToken stoppingToken, int maxExecutionCount)
        {
            while (!stoppingToken.IsCancellationRequested && _executionCount <= maxExecutionCount)
            {
                var propertiesCount = await _context.Properties.Find(_ => true).CountDocumentsAsync();

                _logger.LogInformation("Properties count: {0}. Database query execution count: {1}", propertiesCount, _executionCount);
                await Task.Delay(5_000);
                ++_executionCount;
            }
            return;
        }
    }
}

