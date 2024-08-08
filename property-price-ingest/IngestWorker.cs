using Google.Cloud.PubSub.V1;
using Newtonsoft.Json;
using property_price_api.Models;
using property_price_api.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace property_price_ingest;

public sealed class IngestWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestWorker> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly SubscriberClient _subscriberClient;
    private readonly IIngestJobService _ingestJobService;
    private readonly IHttpClientFactory _httpClientFactory;

    public IngestWorker(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<IngestWorker> logger,
        IHostApplicationLifetime lifeTime,
        SubscriberClient subscriberClient,
        IIngestJobService ingestJobService,
        IHttpClientFactory httpClientFactory
        )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _lifetime = lifeTime;
        _subscriberClient = subscriberClient;
        _ingestJobService = ingestJobService;
        _httpClientFactory = httpClientFactory;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(IngestWorker));
        _logger.LogInformation("Start listening to messages from Cloud Pub/Sub...");
        await _subscriberClient.StartAsync(async (message, _) =>
        {
            var text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
            _logger.LogInformation("Received message from Cloud Pub Sub: {id}", message.MessageId);
            var result = JsonConvert.DeserializeObject<CloudPubSubMessage>(text);
            _logger.LogInformation("Ingest job ID: {jodId}; postcode: {postcode}", result.JobId, result.PostCode);
            var (_, randomNumber) = await GetRandomNumberViaApi(result.PostCode);
            _logger.LogInformation("Random number is: {randomNumber}", randomNumber);
            try
            {

                await _ingestJobService.UpdateIngestJobPriceById(result.JobId, new Random().Next(500_000, 1_000_000));
                _logger.LogInformation("Update job complete: {jodId}", result.JobId);
                return await Task.FromResult(SubscriberClient.Reply.Ack);
            }
            catch (Exception e)
            {
                _logger.LogError("Update job failed: {0} with exception {1}", result.JobId, e.Message);
                return await Task.FromResult(SubscriberClient.Reply.Nack);
            }

        });
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is stopping...", nameof(IngestWorker));
        _logger.LogInformation("Stop listening to messages from Cloud Pub/Sub...");
        await _subscriberClient.StopAsync(stoppingToken);
        await base.StopAsync(stoppingToken);
    }

    private async Task<(string, int)> GetRandomNumberViaApi(string postcode)
    {
        var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.RandomNumberApiHttpClientName);
        var httpResponseMessage = await httpClient.GetAsync(
            "api/v1.0/random?min=100&max=1000");

        if (!httpResponseMessage.IsSuccessStatusCode) return 0;
        await using var contentStream =
            await httpResponseMessage.Content.ReadAsStreamAsync();

        var res = (List<int>)await JsonSerializer.DeserializeAsync
            <IEnumerable<int>>(contentStream);
        return (postcode, res[0]);
    }
}

