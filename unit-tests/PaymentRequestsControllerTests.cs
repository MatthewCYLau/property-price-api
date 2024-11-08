using Microsoft.AspNetCore.Mvc;
using Moq;
using property_price_cosmos_db.Controllers;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace unit_tests;

public class PaymentRequestsControllerTests
{
    [Test]
    public async Task CreateTransactionIdShould()
    {

        var mockPaymentRequestService = new Mock<IPaymentRequestService>();
        // Mock<IConfiguration> mockConfiguration = new();
        var debtorUserId = Guid.NewGuid();
        var creditorUserId = Guid.NewGuid();
        var request = new PaymentRequest { DebtorUserId = debtorUserId, CreditorUserId = creditorUserId, Amount = 10 };
        mockPaymentRequestService.Setup(x => x.CreatePaymentRequest(request)).Returns(Task.FromResult(Result.Failure(PaymentRequestErrors.CreditorAndDebtorIdentical(creditorUserId.ToString(), debtorUserId.ToString()))));
        var paymentRequestsController = new PaymentRequestsController(mockPaymentRequestService.Object);
        var createPaymentRequestResult = await paymentRequestsController.CreatePaymentRequest(request);
        BadRequestObjectResult? result = createPaymentRequestResult as BadRequestObjectResult;

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToString().Contains("PaymentRequestErrors.CreditorAndDebtorIdentical"), Is.True);
        Assert.AreEqual(result.StatusCode, 400);
    }
}
