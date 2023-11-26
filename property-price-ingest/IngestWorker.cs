using property_price_ingest.Services;

namespace property_price_ingest;

public sealed class IngestWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestWorker> _logger;
    private readonly IHostApplicationLifetime _lifetime;

    public IngestWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestWorker> logger,
        IHostApplicationLifetime lifeTime)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _lifetime = lifeTime;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{0} is running...", nameof(IngestWorker));
        using var scope = _serviceScopeFactory.CreateScope();
        var processService = scope.ServiceProvider.GetService<IScopedProcessingService>();
        await processService.GetDataAsync(stoppingToken, 5);
        _lifetime.StopApplication();
       
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{0} is stopping...", nameof(IngestWorker));
        await base.StopAsync(stoppingToken);
    }
}

