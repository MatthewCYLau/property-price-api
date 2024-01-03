using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_ingest;

public sealed class IngestWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestWorker> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly SubscriberClient _subscriberClient;
    private readonly IIngestJobService _ingestJobService;

    public IngestWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestWorker> logger,
        IHostApplicationLifetime lifeTime,
        SubscriberClient subscriberClient,
        IIngestJobService ingestJobService
        )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _lifetime = lifeTime;
        _subscriberClient = subscriberClient;
        _ingestJobService = ingestJobService;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{0} is running...", nameof(IngestWorker));
        _logger.LogInformation("Start listening to messages from Cloud Pub/Sub...");
        await _subscriberClient.StartAsync((message, token) =>
        {
            string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            _logger.LogInformation("Received message from Cloud Pub Sub: {0}", message.MessageId);
            var result = JsonConvert.DeserializeObject<CloudPubSubMessage>(text);
            _logger.LogInformation("Ingest job ID: {0}; postcode: {0}", result.JobId, result.PostCode);
            try
            {
                _ingestJobService.UpdateIngestJobById(result.JobId, new Random().Next(500_000, 1_000_000));
                _logger.LogInformation("Update job complete: {0}", result.JobId);
                return Task.FromResult(SubscriberClient.Reply.Ack);
            }
            catch (Exception e)
            {
                _logger.LogError("Update job failed: {0} with execption {1}", result.JobId, e.Message);
                return Task.FromResult(SubscriberClient.Reply.Nack);
            }
            
        });
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{0} is stopping...", nameof(IngestWorker));
        _logger.LogInformation("Stop listening to messages from Cloud Pub/Sub...");
        await _subscriberClient.StopAsync(stoppingToken);
        await base.StopAsync(stoppingToken);
    }
}

