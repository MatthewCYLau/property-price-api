using System.Net;
using System.Net.Sockets;
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
        TcpListener listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        _logger.LogInformation("Listening on port {0}...", 8080);
        using var scope = _serviceScopeFactory.CreateScope();
        var processService = scope.ServiceProvider.GetService<IScopedProcessingService>();
        await processService.GetDataAsync(stoppingToken, 2);
        _lifetime.StopApplication();
       
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{0} is stopping...", nameof(IngestWorker));
        await base.StopAsync(stoppingToken);
    }
}

