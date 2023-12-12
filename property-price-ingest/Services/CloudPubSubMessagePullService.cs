using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using property_price_ingest.Models;

namespace property_price_ingest.Services
{
    public class CloudPubSubMessagePullService : ICloudPubSubMessagePullService
    {

        private readonly ILogger _logger;
        private readonly CloudPubSubConsumerOptions _options;

        public CloudPubSubMessagePullService(ILogger<CloudPubSubMessagePullService> logger, IOptions<CloudPubSubConsumerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task PullMessagesAsync(CancellationToken stoppingToken)
        {
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_options.GcpProjectId, _options.SubscriptionName);
            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName);


            Task startTask = subscriber.StartAsync((PubsubMessage message, CancellationToken cancel) =>
            {
                string text = System.Text.Encoding.UTF8.GetString(message.Data.ToArray());
                _logger.LogInformation("Received message from Cloud Pub Sub: {0} {1}", message.MessageId, text);
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Start pulling messages from Cloud Pub/Sub...");
                await startTask;
            }

            _logger.LogInformation("Stop Cloud Pub/Sub subscriber...");
            await subscriber.StopAsync(CancellationToken.None);
            return;
        }
    }
}
