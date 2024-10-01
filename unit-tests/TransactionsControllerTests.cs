using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using Moq;
using Newtonsoft.Json;
using property_price_cosmos_db.Controllers;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace unit_tests;

public class TransactionsControllerTests
{
    [Test]
    public async Task GetTransactionsShould()
    {
        IEnumerable<Transaction> transactions = [new Transaction { Id = new Guid(), UserId = new Guid(), Amount = 100, Description = "Test", Completed = false, Comments = [] }];
        var mockTransactionService = new Mock<ITransactionService>();
        Mock<IConfiguration> mockConfiguration = new();
        var mockAzureFactory = new Mock<IAzureClientFactory<ServiceBusSender>>();
        mockTransactionService.Setup(x => x.GetMultipleAsync(false, 100, "asc")).Returns(Task.FromResult(transactions));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, mockAzureFactory.Object);
        var transactionsResult = await transactionsController.List(false, 100, "asc");
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(transactions));
    }

    [Test]
    public async Task GetTransactionByIdShould()
    {
        Transaction transaction = new Transaction { Id = new Guid(), UserId = new Guid(), Amount = 100, Description = "Test", Completed = false, Comments = [] };
        var mockTransactionService = new Mock<ITransactionService>();
        Mock<IConfiguration> mockConfiguration = new();
        var mockAzureFactory = new Mock<IAzureClientFactory<ServiceBusSender>>();
        mockTransactionService.Setup(x => x.GetAsync("1")).Returns(Task.FromResult(transaction));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, mockAzureFactory.Object);
        var transactionsResult = await transactionsController.Get("1");
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(transaction));
    }

    [Test]
    [Ignore("Mocking Sender to be implemented.")]
    public async Task CreateTransactionIdShould()
    {
        string text = File.ReadAllText("resources/example.json");
        var transaction = JsonConvert.DeserializeObject<Transaction>(text);
        var mockTransactionService = new Mock<ITransactionService>();
        var mockAzureFactory = new Mock<IAzureClientFactory<ServiceBusSender>>();
        Mock<IConfiguration> mockConfiguration = new();
        mockTransactionService.Setup(x => x.AddAsync(transaction)).Returns(Task.FromResult(Result.Success()));
        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, mockAzureFactory.Object);
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
        var mockAzureFactory = new Mock<IAzureClientFactory<ServiceBusSender>>();
        mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "MyDatabase")]).Returns("bar");

        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);

        var transactionsController = new TransactionsController(mockTransactionService.Object, mockConfiguration.Object, mockAzureFactory.Object);
        var transactionsResult = await transactionsController.GetSecretFromAzureKeyVault();
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo("bar"));
    }
}