using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using property_price_cosmos_db.Controllers;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace unit_tests;

public class TransactionsControllerTests
{

    private ServiceProvider _serviceProvider;
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
    }
    [Test]
    public async Task GetTransactionsShould()
    {
        IEnumerable<Transaction> transactions = [new Transaction { Id = new Guid(), UserId = new Guid(), Amount = 100, Description = "Test", Completed = false, Comments = [], TransactionType = 0 }];
        var mockTransactionService = new Mock<ITransactionService>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.GetMultipleAsync(false, 100, "asc", 1, 5)).Returns(Task.FromResult(transactions));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.List(false, 100, "asc");
        OkObjectResult? okResult = transactionsResult as OkObjectResult;
        var getTransactionsResponse = okResult.Value as GetTransactionsResponse;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(getTransactionsResponse.Transactions, Is.EqualTo(transactions));
            Assert.That(getTransactionsResponse.TransactionsMetadata.TotalCount, Is.EqualTo(transactions.Count()));
        });
    }

    [Test]
    public async Task GetTransactionByIdShould()
    {
        Transaction transaction = new Transaction { Id = new Guid(), UserId = new Guid(), Amount = 100, Description = "Test", Completed = false, Comments = [], TransactionType = 0 };
        var mockTransactionService = new Mock<ITransactionService>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.GetAsync("1")).Returns(Task.FromResult(transaction));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.Get("1");
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(transaction));
        });
    }

    [Test]
    public async Task DeleteTransactionByIdShould()
    {
        var mockTransactionService = new Mock<ITransactionService>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.DeleteAsync("1"));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.Delete("1");
        NoContentResult? noContentResult = transactionsResult as NoContentResult;

        // Assert
        Assert.IsNotNull(noContentResult);
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    [Test]
    public async Task CreateTransactionIdShould()
    {
        string text = File.ReadAllText("resources/example.json");
        var transaction = JsonConvert.DeserializeObject<Transaction>(text);
        var mockTransactionService = new Mock<ITransactionService>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.AddAsync(transaction)).Returns(Task.FromResult(Result.Success()));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.CreateTransaction(transaction);
        CreatedAtActionResult? result = transactionsResult as CreatedAtActionResult;

        // Assert
        Assert.IsNotNull(text);
        Assert.AreEqual(transaction.Amount, 2000);
        Assert.That(result.Value, Is.EqualTo(transaction));
    }

    [Test]
    public async Task GetSecretShould()
    {
        var mockTransactionService = new Mock<ITransactionService>();
        var mockConfSection = new Mock<IConfigurationSection>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "MyDatabase")]).Returns("bar");

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.GetSecretFromAzureKeyVault();
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("bar"));
        });
    }

    [Test]
    public async Task ReadTransactionBlobShould()
    {
        IEnumerable<Transaction> transactions = [new Transaction { Id = new Guid(), UserId = new Guid(), Amount = 100, Description = "Test", Completed = false, Comments = [], TransactionType = 0 }];
        var mockTransactionService = new Mock<ITransactionService>();
        Mock<IConfiguration> mockConfiguration = new();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        mockTransactionService.Setup(x => x.ReadTransactionBlobAsync("1", "1")).Returns(Task.FromResult(Result<IEnumerable<Transaction>>.Success(transactions)));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var transactionsResult = await transactionsController.ReadTransactionBlobData("1", "1");
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(transactions));
        });
    }

    [Test]
    public async Task GetTransactions_InvalidPageSize_Should()
    {
        IEnumerable<Transaction> transactions = [];
        var mockTransactionService = new Mock<ITransactionService>();
        ILogger<TransactionsController> logger = _serviceProvider.GetRequiredService<ILogger<TransactionsController>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.GetMultipleAsync(false, 100, "asc", 1, 5)).Returns(Task.FromResult(transactions));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, logger);
        var result = await transactionsController.List(false, 100, "asc", 1, 6);
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}