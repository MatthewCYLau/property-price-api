using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
	public class CloudPubSubMessage
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string JobId { get; set; }

        public string PostCode { get; set; }

        public CloudPubSubMessage(string jobId, string postCode)
        {
            JobId = jobId;
            PostCode = postCode;
        }
    }
}

