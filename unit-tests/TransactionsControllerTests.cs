using Microsoft.AspNetCore.Mvc;
using Moq;
using property_price_cosmos_db.Controllers;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace unit_tests;

public class TransactionsControllerTests
{
    [Test]
    public async Task GetTransactionsShould()
    {
        IEnumerable<Transaction> transactions = new Transaction[] { new Transaction("1", 100, "Test", false) };
        var mockTransactionService = new Mock<ITransactionService>();
        mockTransactionService.Setup(x => x.GetMultipleAsync(false, 100)).Returns(Task.FromResult(transactions));
        var transactionsController = new TransactionsController(mockTransactionService.Object);
        var transactionsResult = await transactionsController.List(false, 100);
        OkObjectResult? okResult = transactionsResult as OkObjectResult;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(transactions));
    }
}