using System.Globalization;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using CsvHelper;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using property_price_cosmos_db.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using property_price_cosmos_db.Helper;
using StackExchange.Redis;

namespace property_price_cosmos_db.Services;

public class TransactionService : ITransactionService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private readonly Container _container;
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IAzureClientFactory<BlobServiceClient> _azureBlobServiceClientFactory;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;
    // private readonly IDatabase _redis;
    private readonly Dictionary<string, Transaction> inMemoryCache = [];


    public TransactionService(
        ILogger<TransactionService> logger,
        CosmosClient client,
        IUserService userService,
        IAzureClientFactory<BlobServiceClient> azureBlobServiceClientFactory,
        IOptions<CosmosDbOptions> options,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory
        // IConnectionMultiplexer muxer
        )
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _userService = userService;
        _azureBlobServiceClientFactory = azureBlobServiceClientFactory;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.TransactionsContainerId);
        // _redis = muxer.GetDatabase();
    }

    public async Task<Result> AddAsync(Transaction item)

    {
        var user = _userService.GetUserById(item.UserId.ToString());
        if (user.Result == null)
        {
            _logger.LogWarning("Invalid user ID {id}", item.UserId);
            return Result.Failure(TransactionErrors.InvalidUserId(item.UserId.ToString()));
        }
        await _container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));


        var _sender = _serviceBusSenderFactory.CreateClient("topic-sender");
        var sql_value = "val1";
        string messageBody = JsonConvert.SerializeObject(item);
        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(messageBody))
        {
            ApplicationProperties =
                {
                { "var1", sql_value }

            }
        };
        await _sender.SendMessageAsync(message);
        return Result.Success();
    }

    public async Task<Transaction> UpdateTransactionAppendCommentsAsync(string id, Comment comment)
    {
        List<PatchOperation> patchOperations =
        [
            PatchOperation.Add("/comments/-", new Comment(comment.Description){ Id = Guid.NewGuid()})
        ];

        var response = await _container.PatchItemAsync<Transaction>(id, new PartitionKey(id), patchOperations);
        return response.Resource;
    }

    public async Task DeleteAsync(string id)
    {
        await _container.DeleteItemAsync<Transaction>(id, new PartitionKey(id));
    }

    public async Task<Transaction?> GetAsync(string id)
    {
        _logger.LogInformation("Getting transaction for ID {id}", id);

        if (inMemoryCache.TryGetValue(id, out Transaction? value))
        {
            return value;
        }
        else
        {
            try
            {
                // string json;
                // json = await _redis.StringGetAsync(id);
                // if (string.IsNullOrEmpty(json))
                // {
                //     var response = await _container.ReadItemAsync<Transaction>(id, new PartitionKey(id));
                //     string jsonString = JsonConvert.SerializeObject(response.Resource);
                //     var setTask = _redis.StringSetAsync(id, jsonString);
                //     var expireTask = _redis.KeyExpireAsync(id, TimeSpan.FromSeconds(3600));
                //     await Task.WhenAll(setTask, expireTask);
                //     return response.Resource;

                // }
                // var transaction = JsonConvert.DeserializeObject<Transaction>(json);
                // return transaction;
                var response = await _container.ReadItemAsync<Transaction>(id, new PartitionKey(id));
                var transaction = response.Resource;
                inMemoryCache[id] = transaction;
                return transaction;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }

    public async Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount, string? orderBy, int page, int pageSize)
    {
        QueryDefinition queryDefinition;
        string order;
        int offset = (page - 1) * pageSize;

        if (orderBy != null && orderBy == "asc")
        {
            order = "ASC";
        }
        else
        {
            order = "DESC";
        }

        if (isComplete is null)
        {
            queryDefinition = new QueryDefinition(
                query: $"SELECT * FROM transactions ORDER BY transactions.created {order} OFFSET {offset} LIMIT {pageSize}"
            );
        }
        else
        {
            queryDefinition = new QueryDefinition(
                    query: $"SELECT * FROM transactions t WHERE t.isComplete = @isComplete ORDER BY t.created {order} OFFSET {offset} LIMIT {pageSize}"
            )
            .WithParameter("@isComplete", isComplete);
        }
        var query = _container.GetItemQueryIterator<Transaction>(queryDefinition: queryDefinition);

        var results = new List<Transaction>();
        TimeSpan cumulativeTime = new();
        long documentCount = 0;

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            ServerSideCumulativeMetrics metrics = response.Diagnostics.GetQueryMetrics();
            cumulativeTime = metrics.CumulativeMetrics.TotalTime;
            documentCount = metrics.CumulativeMetrics.RetrievedDocumentCount;
            results.AddRange([.. response]);
        }

        if (maxAmount is not null)
        {
            return
                from i in results
                where i.Amount < maxAmount
                select i;
        }

        _logger.LogInformation("Query retrieved {count} documents in {timeSpan} seconds.", documentCount, cumulativeTime.TotalSeconds);
        return results;
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByCommentsCount(int maxCount)
    {
        QueryDefinition queryDefinition = new QueryDefinition(
            query: $"SELECT * FROM Transactions t WHERE ARRAY_LENGTH(t.comments) > @maxCount"
            )
            .WithParameter("@maxCount", maxCount);

        var query = _container.GetItemQueryIterator<Transaction>(queryDefinition: queryDefinition);

        var results = new List<Transaction>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();

            results.AddRange([.. response]);
        }
        return results;
    }

    public async Task<Transaction> UpdateAsync(string id, UpdateTransactionRequest request)
    {

        List<PatchOperation> patchOperations =
    [
    PatchOperation.Set("/amount", request.Amount),
       PatchOperation.Set("/description", request.Description),
       PatchOperation.Set("/isComplete", request.Completed),
       PatchOperation.Set("/modified", DateTime.Now)
    ];

        var response = await _container.PatchItemAsync<Transaction>(id, new PartitionKey(id), patchOperations);
        return response.Resource;
    }

    public async Task<Transaction> UpdateCommentAsync(string transactionId, string commentId, UpdateCommentRequest request)
    {
        var transaction = await GetAsync(transactionId);
        var index = transaction.Comments.FindIndex(n => n.Id == new Guid(commentId));
        var response = await _container.PatchItemAsync<Transaction>(
    transactionId,
    new PartitionKey(transactionId),
    patchOperations: [PatchOperation.Add($"/comments/{index}/Description", request.Description)]);
        return response.Resource;

    }

    public async Task<Transaction> DeleteCommentAsync(string transactionId, string commentId)
    {
        var transaction = await GetAsync(transactionId);
        var updatedComments = transaction.Comments.Where(n => n.Id != new Guid(commentId));
        var response = await _container.PatchItemAsync<Transaction>(
    transactionId,
    new PartitionKey(transactionId),
    patchOperations: [PatchOperation.Replace($"/comments", updatedComments)]);
        return response.Resource;

    }

    public async Task<AnalysisResponse> GetTransactionsAnalysisResponse()
    {

        QueryDefinition count = new(query: "SELECT VALUE COUNT(t.amount) FROM transactions t");
        var countQuery = _container.GetItemQueryIterator<int>(queryDefinition: count);

        var countResults = new List<int>();
        while (countQuery.HasMoreResults)
        {
            var response = await countQuery.ReadNextAsync();
            countResults.AddRange([.. response]);
        }

        QueryDefinition sum = new(query: "SELECT VALUE SUM(t.amount) FROM transactions t");
        var sumQuery = _container.GetItemQueryIterator<decimal>(queryDefinition: sum);

        var sumResults = new List<decimal>();
        while (sumQuery.HasMoreResults)
        {
            var response = await sumQuery.ReadNextAsync();
            sumResults.AddRange([.. response]);
        }

        QueryDefinition average = new(query: "SELECT VALUE AVG(t.amount) FROM transactions t");
        var averageQuery = _container.GetItemQueryIterator<decimal>(queryDefinition: average);

        var averageResults = new List<decimal>();
        while (averageQuery.HasMoreResults)
        {
            var response = await averageQuery.ReadNextAsync();
            averageResults.AddRange([.. response]);
        }

        return new AnalysisResponse { Count = countResults[0], Sum = Math.Round(sumResults[0], 2), Average = Math.Round(averageResults[0], 2) };
    }

    public async Task<Transaction> UpdateTrasnscationCompleteState(string id, bool isComplete)
    {
        List<PatchOperation> patchOperations =
    [
       PatchOperation.Set("/isComplete", isComplete),
];

        var response = await _container.PatchItemAsync<Transaction>(id, new PartitionKey(id), patchOperations);
        return response.Resource;
    }

    public async Task<Result<IEnumerable<Transaction>>> ReadTransactionBlobAsync(string transactionId, string blobId)
    {
        var transaction = await GetAsync(transactionId);
        var blobServiceClient = _azureBlobServiceClientFactory.CreateClient("main");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(transaction.UserId.ToString());
        BlobClient blobClient = blobContainerClient.GetBlobClient($"transactions-export-{blobId}.csv");
        using var memoryStream = new MemoryStream();

        try
        {
            blobClient.DownloadToAsync(memoryStream).GetAwaiter().GetResult();
            memoryStream.Position = 0;
            using var reader = new StreamReader(memoryStream);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            var transactions = csv.GetRecords<Transaction>().ToList();
            foreach (Transaction t in transactions)
            {
                _logger.LogInformation("Reading transaction wit id {} and amount {}", t.Id, t.Amount);
            }
            return Result<IEnumerable<Transaction>>.Success(transactions);
        }
        catch (Exception e)
        {
            _logger.LogError("Download blob failed against blob ID {0} with exception {1}", blobId, e.Message);
            return Result<IEnumerable<Transaction>>.Failure(BlobClientErrors.InvalidBlobId(blobId));
        }
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByUserId(string id)
    {
        QueryDefinition queryDefinition = new QueryDefinition(
                    query: $"SELECT * FROM transactions t WHERE t.userId = @userId"
            )
            .WithParameter("@userId", id);
        var query = _container.GetItemQueryIterator<Transaction>(queryDefinition: queryDefinition);

        var results = new List<Transaction>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange([.. response]);
        }

        return results;
    }

    private static async Task<UserDelegationKey> RequestUserDelegationKey(
    BlobServiceClient blobServiceClient)
    {
        UserDelegationKey userDelegationKey =
            await blobServiceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddDays(1));

        return userDelegationKey;
    }

    private static async Task<Uri> CreateUserDelegationSASBlob(
    BlobClient blobClient,
    UserDelegationKey userDelegationKey)
    {
        // Create a SAS token for the blob resource that's also valid for 1 day
        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = blobClient.BlobContainerName,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Specify the necessary permissions
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        // Add the SAS token to the blob URI
        BlobUriBuilder uriBuilder = new(blobClient.Uri)
        {
            // Specify the user delegation key
            Sas = sasBuilder.ToSasQueryParameters(
                userDelegationKey,
                blobClient
                .GetParentBlobContainerClient()
                .GetParentBlobServiceClient().AccountName)
        };

        return uriBuilder.ToUri();
    }

    public async Task<Uri> ExportTransactionsByUserId(string id)
    {
        var transactions = await GetTransactionsByUserId(id);
        var blobServiceClient = _azureBlobServiceClientFactory.CreateClient("main");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(id);
        var csvName = $"transactions-export-{DateTime.Now.ToFileTime()}.csv";
        BlobClient blobClient = blobContainerClient.GetBlobClient(csvName);

        using (var writer = new StringWriter())
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(transactions);
            string csvContent = writer.ToString();

            Stream streamToUploadToBlob = StreamHelper.GenerateStreamFromString(csvContent);
            await blobClient.UploadAsync(streamToUploadToBlob);
        }
        _logger.LogInformation("Uploaded CSV with name {csvName}", csvName);

        var key = await RequestUserDelegationKey(blobServiceClient);
        var url = await CreateUserDelegationSASBlob(blobClient, key);
        return url;
    }

    private async Task<Guid> GetSeedUserId()
    {
        var users = await _userService.GetUsers(null, null);
        if (!users.Any())
        {
            _logger.LogInformation("Creating seed user...");
            await _userService.AddUserAsync(CosmosUserMap.CosmosUsers[1]);
            users = await _userService.GetUsers(null, null);
            return users.First().Id;
        }
        else
        {
            return users.First().Id;
        }
    }

    public async Task CreateSeedTransactions()
    {

        var testUserId = await GetSeedUserId();
        var transactions = await GetMultipleAsync(null, 1_000_000, "asc", 1, 5);
        if (!transactions.Any())
        {
            _logger.LogInformation("Creating seed transaction...");
            var seedTransactions = new List<Transaction>
{
  new() { Id = Guid.NewGuid(), UserId = testUserId, Amount = 100, Description = "Seed transaction", Completed = false, Comments = [], TransactionType = (TransactionType)1 }
};
            seedTransactions.ForEach(async t =>
            {
                await AddAsync(t);
            });
        }
        else
        {
            _logger.LogInformation("Skip creating seed transactions. Current transactions count :{count}; count is even number? {isEven}", transactions.Count(), MathsHelper.IsEven(transactions.Count()));
        }

    }

    public async Task<Result<IEnumerable<Comment>>> GetCommentsByTransactionId(string id)
    {

        var transaction = GetAsync(id);
        if (transaction.Result == null)
        {
            _logger.LogWarning("Invalid transaction ID {id}", id);
            return Result<IEnumerable<Comment>>.Failure(TransactionErrors.InvalidTransactionId(id));
        }
        _logger.LogInformation("Getting comments by transaction ID {id}", id);
        var queryDefinition = new QueryDefinition(
                query: $"SELECT c.Id, c.Description FROM transactions t JOIN c IN t.comments WHERE t.id = @transactionId"
        )
        .WithParameter("@transactionId", id);
        var query = _container.GetItemQueryIterator<Comment>(queryDefinition: queryDefinition);

        var results = new List<Comment>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange([.. response]);
        }
        return Result<IEnumerable<Comment>>.Success(results);
    }
}