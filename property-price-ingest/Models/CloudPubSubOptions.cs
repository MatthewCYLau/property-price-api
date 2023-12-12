namespace property_price_ingest.Models
{
	public class CloudPubSubConsumerOptions
    {
        public const string CloudPubSub = "CloudPubSub";

        public string SubscriptionName { get; set; } = string.Empty;
        public string GcpProjectId { get; set; } = string.Empty;
    }
}

