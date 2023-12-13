namespace property_price_api.Models
{
	public class CreateJobResponse
	{
        public string MessageId { get; set; }
        public string IngestJobId { get; set; }

        public CreateJobResponse(string messageId, string ingestJobId)
        {
            MessageId = messageId;
            IngestJobId = ingestJobId;
        }
    }
}

