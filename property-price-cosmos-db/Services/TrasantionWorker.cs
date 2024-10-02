using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

namespace property_price_cosmos_db.Services;

public class TrasantionWorker : BackgroundService, IAsyncDisposable
{

    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private readonly ILogger<TrasantionWorker> _logger;
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;


    public TrasantionWorker(ILogger<TrasantionWorker> logger, IAzureClientFactory<ServiceBusClient> serviceBusClientFactory)
    {
        _logger = logger;
        _serviceBusClientFactory = serviceBusClientFactory;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(TrasantionWorker));
        var _client = _serviceBusClientFactory.CreateClient("main");
        _processor = _client.CreateProcessor("sbt-aks-storage-request", "aks-storage-request", new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{worker} is stopping...", nameof(TrasantionWorker));
        // await _processor.StopProcessingAsync();
        // await base.StopAsync(cancellationToken);
    }


    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        _logger.LogInformation("####### Received message from Service Bus {message} #######", body);
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception.ToString());
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
        // await _client.DisposeAsync();
    }
}
