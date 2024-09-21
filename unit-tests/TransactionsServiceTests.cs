using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace unit_tests;

[Ignore("Test to be ran locally")]
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
            options.DatabaseId = "TransactionsDb";
            options.TransactionsContainerId = "TransasctionsContainer";
        });
        services.AddSingleton<CosmosClient>(_ => new CosmosClient(""));
        services.AddSingleton<ITransactionService, TransactionService>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task GetTransactions()
    {
        var transactionService = _serviceProvider.GetService<ITransactionService>();
        var transactions = await transactionService.GetMultipleAsync(false, 100, "asc");
        Console.WriteLine("Retrieved {0} transactions from database.", transactions.Count());
        Assert.That(transactions.Count() > 0);
    }
}