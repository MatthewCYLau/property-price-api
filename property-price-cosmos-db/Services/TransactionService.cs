using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public class TransactionService: ITransactionService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private Container _container;
    private readonly ILogger _logger;
    
    public TransactionService(
        ILogger<TransactionService> logger,
        CosmosClient client,
        IOptions<CosmosDbOptions> options)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _container = _client.GetContainer(_options.DatabaseId, _options.ContainerId);
    }
    
    public async Task AddAsync(Transaction item)
    
    {
        await _container.CreateItemAsync(item, new PartitionKey(item.Id));
    }

    public async Task<Transaction> UpdateTransactionCommentsAsync(string id, Comment comment)
    {
        List<PatchOperation> patchOperations = new List<PatchOperation>
        {
            PatchOperation.Replace("/comments",  new List<Comment>
            {
                new(Guid.NewGuid().ToString(), comment.Description)
            })
        };
        
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

    public async Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount)
    {
        QueryDefinition queryDefinition;
        
        if (isComplete is null)
        {
            queryDefinition = new QueryDefinition(
                query: "SELECT * FROM transactions"
            );
        }
        else
        {
            queryDefinition = new QueryDefinition(
                    query: "SELECT * FROM transactions t WHERE t.isComplete = @isComplete"
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

    public async Task<Transaction> UpdateAsync(string id, Transaction item)
    {
        var response = await _container.UpsertItemAsync(item, new PartitionKey(id));
        return response.Resource;
    }
}