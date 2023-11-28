using System.Diagnostics;
using System.Text.Json;
using MongoDB.Driver;
using property_price_api.Data;
using property_price_api.Models;
using property_price_api.Helpers;

namespace property_price_ingest.Services
{
	public class ScopedProcessingService : IScopedProcessingService
    {
        private int _executionCount = 1;
        private readonly ILogger<ScopedProcessingService> _logger;
        private readonly MongoDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;


        public ScopedProcessingService(
            ILogger<ScopedProcessingService> logger,
            MongoDbContext context,
            IHttpClientFactory httpClientFactory
            )
        {
            _logger = logger;
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task GetDataAsync(CancellationToken stoppingToken, int maxExecutionCount)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (!stoppingToken.IsCancellationRequested && _executionCount <= maxExecutionCount)
            {
                var propertiesCount = await _context.Properties.Find(_ => true).CountDocumentsAsync();

                _logger.LogInformation("Properties count: {0}. Database query execution count: {1}", propertiesCount, _executionCount);

                var posts = GetJsonPlaceholderPosts();

                foreach (Post post in posts.Result)
                {
                    _logger.LogInformation(post.body);
                    string[] copies = post.body.Split(' ');
                    var upperCaseWords = copies.Select(StringHelpers.UpperCaseWord);
                    var output = string.Join(" ", upperCaseWords);
                    _logger.LogInformation(output);
                }

                await Task.Delay(5_000);
                ++_executionCount;
            }
            _logger.LogInformation("Operation complete. Time taken in seconds: {0}", timer.Elapsed.TotalSeconds);
            return;
        }

        private async Task<List<Post>> GetJsonPlaceholderPosts()
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.jsonPlaceholderHttpClientName);
            var httpResponseMessage = await httpClient.GetAsync(
                "posts?_start=0&_limit=2");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                return (List<Post>)await JsonSerializer.DeserializeAsync
                   <IEnumerable<Post>>(contentStream);
            }

            return new List<Post>();
        }
    }
}

