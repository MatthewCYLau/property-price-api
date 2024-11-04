using System.Text;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public class EventProcessorService(
    IAzureClientFactory<BlobServiceClient> blobServiceClientFactory,
    ILogger<EventProcessorService> logger,
    IOptions<ManagedIdentityOptions> options,
    IConfiguration configuration
        ) : BackgroundService, IAsyncDisposable
{

    private readonly IAzureClientFactory<BlobServiceClient> _blobServiceClientFactory = blobServiceClientFactory;
    private readonly ILogger<EventProcessorService> _logger = logger;
    private readonly IOptions<ManagedIdentityOptions> _options = options;


    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var blobServiceClient = _blobServiceClientFactory.CreateClient("main");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("event-hub-sink");
        var eventHubsNamespace = configuration.GetValue<string>("Azure:EventHubs:Namespace");
        var eventHubName = configuration.GetValue<string>("Azure:EventHubs:EventHubName");
        var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = _options.Value.CliendId
    });
        var eventProcessorClient = new EventProcessorClient(
              blobContainerClient, EventHubConsumerClient.DefaultConsumerGroupName,
              eventHubsNamespace,
              eventHubName, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "development" ? new DefaultAzureCredential() : credential);
        eventProcessorClient.ProcessEventAsync += ProcessEventHandler;
        eventProcessorClient.ProcessErrorAsync += ProcessErrorHandler;
        await eventProcessorClient.StartProcessingAsync(stoppingToken).ConfigureAwait(false);
    }


    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        _logger.LogInformation("Received event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));
        await Task.CompletedTask;
    }

    private async Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        _logger.LogError(eventArgs.PartitionId);
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {

    }
}
