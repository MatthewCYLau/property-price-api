using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

namespace unit_tests;

[TestFixture]
[Category("Integration")]
public class TransactionsServiceTests
{
    private const string TestTransactionId = "076e49b3-9295-4c88-b739-bd0cf3b1c628";
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        services.Configure<CosmosDbOptions>(options =>
        {
            options.DatabaseId = "transactionsdb";
            options.TransactionsContainerId = "transactions";
            options.UsersContainerId = "users";
        });
        services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? settings.ConnectionString);
});
        services.AddSingleton<ITransactionService, TransactionService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient("foo").WithName("main");
            clientBuilder.AddServiceBusClientWithNamespace("foo").WithName("main");
            clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                        provider
                        .GetService<IAzureClientFactory<ServiceBusClient>>()
                        .CreateClient("main")
                        .CreateSender("foo")
                    ).WithName("topic-sender");
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task GetTransactionsShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(false, 100, "asc");
        Assert.That(transactions.Count(), Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTransactionByIdShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transaction = await transactionService.GetAsync(TestTransactionId);
        Assert.That(transaction.Id.ToString(), Is.EqualTo(TestTransactionId));
    }
}