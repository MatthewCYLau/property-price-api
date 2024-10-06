using System.Globalization;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using CsvHelper;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace property_price_cosmos_db.Services;

public class TransactionService : ITransactionService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private Container _container;
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactory;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;


    public TransactionService(
        ILogger<TransactionService> logger,
        CosmosClient client,
        IUserService userService,
        IAzureClientFactory<BlobServiceClient> azureClientFactory,
        IOptions<CosmosDbOptions> options,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory
        )
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _userService = userService;
        _azureClientFactory = azureClientFactory;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.TransactionsContainerId);
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


        var _sender = _serviceBusSenderFactory.CreateClient("sender");
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

        var blobServiceClient = _azureClientFactory.CreateClient("main");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(item.UserId.ToString());
        BlobClient blobClient = blobContainerClient.GetBlobClient($"{item.Id}.csv");

        using (var writer = new StringWriter())
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(new Transaction[] { item });
            string csvContent = writer.ToString();

            Stream streamToUploadToBlob = GenerateStreamFromString(csvContent);
            await blobClient.UploadAsync(streamToUploadToBlob);
        }
        _logger.LogInformation("Uploaded CSV for transaction with ID {id}", item.Id);
        return Result.Success();
    }

    public async Task<Transaction> UpdateTransactionAppendCommentsAsync(string id, Comment comment)
    {
        List<PatchOperation> patchOperations =
        [
            PatchOperation.Add("/comments/-", new Comment(Guid.NewGuid(), comment.Description))
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
        try
        {
            var response = await _container.ReadItemAsync<Transaction>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount, string? orderBy)
    {
        QueryDefinition queryDefinition;
        string order;

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
                query: $"SELECT * FROM transactions ORDER BY transactions.created {order}"
            );
        }
        else
        {
            queryDefinition = new QueryDefinition(
                    query: $"SELECT * FROM transactions t WHERE t.isComplete = @isComplete ORDER BY t.created {order}"
            )
            .WithParameter("@isComplete", isComplete);
        }
        var query = _container.GetItemQueryIterator<Transaction>(queryDefinition: queryDefinition);

        var results = new List<Transaction>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        if (maxAmount is not null)
        {
            return
                from i in results
                where i.Amount < maxAmount
                select i;
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

    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
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

    public async Task<IEnumerable<Transaction>> ReadTransactionBlobAsync(string transactionId)
    {
        var transaction = await GetAsync(transactionId);
        var blobServiceClient = _azureClientFactory.CreateClient("main");
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(transaction.UserId.ToString());
        BlobClient blobClient = blobContainerClient.GetBlobClient($"{transaction.Id}.csv");
        using var memoryStream = new MemoryStream();
        blobClient.DownloadToAsync(memoryStream).GetAwaiter().GetResult();
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
        var transactions = csv.GetRecords<Transaction>().ToList();
        foreach (Transaction t in transactions)
        {
            _logger.LogInformation("Reading transaction wit id {} and amount {}", t.Id, t.Amount);
        }
        return transactions;
    }
}