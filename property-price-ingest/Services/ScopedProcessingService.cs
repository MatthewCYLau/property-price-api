using MongoDB.Driver;
using property_price_api.Data;

namespace property_price_ingest.Services
{
	public class ScopedProcessingService : IScopedProcessingService
    {
        private int _executionCount;
        private readonly ILogger<ScopedProcessingService> _logger;
        private readonly MongoDbContext _context;

        public ScopedProcessingService(
            ILogger<ScopedProcessingService> logger,
            MongoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && _executionCount < 2)
            {
                ++_executionCount;

                _logger.LogInformation(
                    "{ServiceName} working, execution count: {Count}",
                    nameof(ScopedProcessingService),
                _executionCount);

                var properties = await _context.Users.Find(_ => true).ToListAsync();

                properties.ForEach(n => _logger.LogInformation(n.Email));
                await Task.Delay(5_000, stoppingToken);
            }
        }
    }
}

