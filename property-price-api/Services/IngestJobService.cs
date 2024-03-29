﻿using System.Linq.Expressions;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;

namespace property_price_api.Services
{
    public interface IIngestJobService
    {
        Task<List<IngestJob>> GetIngestJobs(bool complete = false, string postcode = "");
        Task<string> CreateIngestJob(string postcode);
        Task<bool> UpdateIngestJobPriceById(string id, int transactionPrice);
        Task<IngestJob> GetIngestJobById(string? id);
        Task DeleteIngestJobById(string id);
    }

    public class IngestJobService : IIngestJobService
    {

        private readonly MongoDbContext _context;
        private static readonly int DAILY_LIMIT = 500;
        private readonly ILogger _logger;

        public IngestJobService(
            MongoDbContext context,
            ILogger<IngestJobService> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateIngestJob(string postcode)
        {

            var count = await GetIngestJobsCreatedTodayCount();

            if (count > DAILY_LIMIT)
            {
                throw new CustomException("Ingest job creation daily limit reached");
            }

            var job = new IngestJob(postcode)
            {
                Created = DateTime.Now
            };
            await _context.IngestJobs.InsertOneAsync(job);
            return job.Id;
        }

        public async Task<IngestJob> GetIngestJobById(string? id)
        {
            var ingestJob = await _context.IngestJobs.Find(x => x.Id == id).FirstOrDefaultAsync();
            return ingestJob;
        }

        public async Task<bool> UpdateIngestJobPriceById(string id, int transactionPrice)
        {
            var filter = Builders<IngestJob>.Filter.Where(x => x.Id == id);
            var update = Builders<IngestJob>.Update
                .Set(x => x.TransactionPrice, transactionPrice)
                .Set(x => x.Complete, true);

            var options = new FindOneAndUpdateOptions<IngestJob>();
            await _context.IngestJobs.FindOneAndUpdateAsync(filter, update, options);

            return true;
        }

        private async Task<long> GetIngestJobsCreatedTodayCount()
        {
            Expression<Func<IngestJob, bool>> expression = x => x.Created > DateTime.Today;
            var count = await _context.IngestJobs.CountDocumentsAsync(Builders<IngestJob>.Filter.Where(expression));
            _logger.LogWarning("Ingest jobs created today count: {0}", count);
            return count;
        }

        public async Task<List<IngestJob>> GetIngestJobs(bool complete = false, string postcode = "")
        {

            Expression<Func<IngestJob, bool>> expression;
            if (string.IsNullOrEmpty(postcode))
            {
                expression = x => x.Complete == complete;
            }
            else
            {   
                expression = x => x.Postcode == postcode && x.Complete == complete;
            }

            return await _context.IngestJobs.Find(expression).ToListAsync();
        }

        public async Task DeleteIngestJobById(string id) =>
         await _context.IngestJobs.DeleteOneAsync(x => x.Id == id);
    }
}

