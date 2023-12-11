namespace property_price_api.Models
{
	public class CloudPubSubOptions
	{
        public const string CloudPubSub = "CloudPubSub";

        public string TopicName { get; set; } = string.Empty;
        public string GcpProjectId { get; set; } = string.Empty;
    }
}

