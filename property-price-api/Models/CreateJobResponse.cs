namespace property_price_api.Models
{
    public class CreateJobResponse(string messageId, string ingestJobId)
    {
        public string MessageId { get; set; } = messageId;
        public string IngestJobId { get; set; } = ingestJobId;
    }
}

