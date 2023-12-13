using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IIngestJobService
    {
        Task<string> CreateIngestJob(string postcode);
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
    }
}

