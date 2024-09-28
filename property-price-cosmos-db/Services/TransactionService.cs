using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public class TransactionService : ITransactionService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private Container _container;
    private readonly ILogger _logger;
    private readonly IUserService _userService;
    private readonly IAzureClientFactory<BlobServiceClient> _azureClientFactory;

    public TransactionService(
        ILogger<TransactionService> logger,
        CosmosClient client,
        IUserService userService,
        IAzureClientFactory<BlobServiceClient> azureClientFactory,
        IOptions<CosmosDbOptions> options)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _userService = userService;
        _azureClientFactory = azureClientFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.TransactionsContainerId);
    }

    public async Task AddAsync(Transaction item)

    {
        var user = _userService.GetUserById(item.UserId.ToString());
        if (user.Result == null)
        {
            _logger.LogWarning("Invalid user ID {id}", item.UserId);
        }

        var blobServiceClient = _azureClientFactory.CreateClient("main");
        await _container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));
        BlobContainerClient container = await blobServiceClient.CreateBlobContainerAsync(item.Id.ToString());
        if (await container.ExistsAsync())
        {
            _logger.LogInformation("Created container {0}", container.Name);
        }
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

}