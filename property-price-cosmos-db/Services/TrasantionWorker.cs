using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace property_price_cosmos_db.Services;

public class TrasantionWorker : BackgroundService, IAsyncDisposable
{

    // private ServiceBusClient _client;
    // private ServiceBusProcessor _processor;
    private readonly ILogger<TrasantionWorker> _logger;

    public TrasantionWorker(ILogger<TrasantionWorker> logger)
    {
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(TrasantionWorker));
        // _client = new ServiceBusClient("myConnection");
        // _processor = _client.CreateProcessor("myQueue", new ServiceBusProcessorOptions());
        // _processor.ProcessMessageAsync += MessageHandler;
        // await _processor.StartProcessingAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{worker} is stopping...", nameof(TrasantionWorker));
        // await _processor.StopProcessingAsync();
        // await base.StopAsync(cancellationToken);
    }


    private static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        // string body = args.Message.Body.ToString();
        // await args.CompleteMessageAsync(args.Message);
    }

    public async ValueTask DisposeAsync()
    {
        // await _processor.DisposeAsync();
        // await _client.DisposeAsync();
    }
}
