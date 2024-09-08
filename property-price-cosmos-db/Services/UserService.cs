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
}
