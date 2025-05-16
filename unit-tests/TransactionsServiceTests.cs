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
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient("foo").WithName("main");
            clientBuilder.AddServiceBusClientWithNamespace("foo").WithName("main");
            // clientBuilder.AddEventHubProducerClientWithNamespace("foo", "example").WithName("event-hub-producer");
            clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                        provider
                        .GetService<IAzureClientFactory<ServiceBusClient>>()
                        .CreateClient("main")
                        .CreateSender("foo")
                    ).WithName("topic-sender");
        });

        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task GetTransactionsShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(null, 1_000_000, "asc", 1, 5);
        Assert.That(transactions.Count(), Is.GreaterThan(0));
    }

    [Test]
    public async Task GetTransactionByIdShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(null, 1_000_000, "asc", 1, 5);
        var firstTransactionId = transactions.ElementAt(0).Id.ToString();
        var transaction = await transactionService.GetAsync(firstTransactionId);
        Assert.That(transaction.Id.ToString(), Is.EqualTo(firstTransactionId));
    }

    [Test]
    public async Task UpdateTransactionByIdShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(null, 1_000_000, "asc", 1, 5);
        var firstTransactionId = transactions.ElementAt(0).Id.ToString();
        var updatedTransaction1 = await transactionService.UpdateAsync(firstTransactionId, new UpdateTransactionRequest() { Amount = 200, Description = "example", Completed = false });
        Assert.That(updatedTransaction1.Amount, Is.EqualTo(200));
        var updatedTransaction2 = await transactionService.UpdateAsync(firstTransactionId, new UpdateTransactionRequest() { Amount = 100, Description = "example", Completed = false });
        Assert.That(updatedTransaction2.Amount, Is.EqualTo(100));
    }

    [Test]
    [Ignore("Avoid CosmosDB rate limitting")]
    public async Task UpdateTransactionAppendCommentsAsyncShould()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(null, 1_000_000, "asc", 1, 5);
        var firstTransactionId = transactions.ElementAt(0).Id.ToString();
        await transactionService.UpdateTransactionAppendCommentsAsync(firstTransactionId, new Comment("example"));
        var transaction = await transactionService.GetAsync(firstTransactionId);
        Assert.That(transaction.Comments.Count, Is.GreaterThan(0));
        Assert.That(transaction.Comments[0].Description.Length, Is.GreaterThan(0));
    }

    [TearDown]
    public async Task TeardownAsync()
    {

        ILogger<TransactionService> _logger = _serviceProvider.GetRequiredService<ILogger<TransactionService>>();
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetTransactionsByCommentsCount(0);
        Assert.That(transactions.Count(), Is.GreaterThan(0));
        var count = 0;
        foreach (var transaction in transactions)
        {
            // await transactionService.DeleteAsync(transaction.Id.ToString());
            count += 1;
        }
        _logger.LogInformation("Deleted {0} transactions as part of clean-up.", count);
    }
}