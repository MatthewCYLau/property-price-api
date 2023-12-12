using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;
using property_price_api.Models;

namespace property_price_api.Services
{

	public interface ICloudPubSubService
	{
		Task<string> PublishMessagesAsync(CloudPubSubMessage message);
	}

    public class CloudPubSubService: ICloudPubSubService
	{

        private readonly ILogger _logger;
        private readonly CloudPubSubOptions _options;

        public CloudPubSubService(ILogger<CloudPubSubService> logger, IOptions<CloudPubSubOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<string> PublishMessagesAsync(CloudPubSubMessage message)
        {

            TopicName topicName = TopicName.FromProjectTopic(_options.GcpProjectId, _options.TopicName);
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            try
            {
                string publishedMessage = await publisher.PublishAsync(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                _logger.LogInformation("Published message to Cloud PubSub - {0}", publishedMessage);
                return publishedMessage;

            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to publish message to Cloud PubSub - {0}", exception.Message);
                throw new CustomException(string.Format("Failed to publish message to Cloud PubSub - {0}", exception.Message));
            }

        }
	}
}

