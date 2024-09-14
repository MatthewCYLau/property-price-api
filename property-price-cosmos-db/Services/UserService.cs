using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public class UserService : IUserService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private Container _container;
    private readonly ILogger _logger;

    public UserService(
        ILogger<UserService> logger,
        CosmosClient client,
        IOptions<CosmosDbOptions> options)
    {
        _client = client;
        _logger = logger;
        _options = options.Value;
        _container = _client.GetContainer(_options.DatabaseId, _options.UsersContainerId);
    }

    public async Task AddUserAsync(CosmosUser item)

    {
        await _container.CreateItemAsync(item, new PartitionKey(item.Id.ToString()));
    }

    public async Task<IEnumerable<CosmosUser>> GetUsers(DateTime? fromDate, DateTime? toDate)
    {

        QueryDefinition queryDefinition;

        if (fromDate == null || toDate == null)
        {
            queryDefinition = new QueryDefinition(
                query: "SELECT * FROM users"
            );
        }
        else
        {
            queryDefinition = new QueryDefinition(
                    query: "SELECT * FROM users u WHERE (u.dateOfBirth >= @fromDate AND u.dateOfBirth <= @toDate)"
            )
            .WithParameter("@fromDate", fromDate).WithParameter("@toDate", toDate);
        }

        var query = _container.GetItemQueryIterator<CosmosUser>(queryDefinition: queryDefinition);

        var results = new List<CosmosUser>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<bool> DeleteUserById(string id)
    {
        try
        {
            await _container.DeleteItemAsync<CosmosUser>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
