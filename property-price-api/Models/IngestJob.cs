using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace property_price_api.Models
{
    public class IngestJob : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Postcode { get; set; }

        public int TransactionPrice { get; set; } = 0;

        public bool Complete { get; set; } = false;

        public IngestJob(string postcode)
        {
            Postcode = postcode;
        }

        public IngestJobStatus IngestJobStatus { get; set; } = IngestJobStatus.InProgress;

    }
}

