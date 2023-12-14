using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IIngestJobService
    {
        Task<string> CreateIngestJob(string postcode);
        Task<bool> UpdateIngestJobById(string id, int transactionPrice);

    }

    public class IngestJobService : IIngestJobService
    {

        private readonly MongoDbContext _context;

        public IngestJobService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateIngestJob(string postcode)
        {
            var job = new IngestJob(postcode)
            {
                Created = DateTime.Now
            };
            await _context.IngestJobs.InsertOneAsync(job);
            return job.Id;
        }

        public async Task<bool> UpdateIngestJobById(string id, int transactionPrice)
        {
            var filter = Builders<IngestJob>.Filter.Where(x => x.Id == id);
            var update = Builders<IngestJob>.Update
                .Set(x => x.TransactionPrice, transactionPrice);

            var options = new FindOneAndUpdateOptions<IngestJob>();
            await _context.IngestJobs.FindOneAndUpdateAsync(filter, update, options);

            return true;
        }
    }
}

