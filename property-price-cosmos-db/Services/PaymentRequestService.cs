using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Globalization;

namespace property_price_cosmos_db.Services;

public class PaymentRequestService : IPaymentRequestService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private readonly Container _container;
    private readonly ILogger _logger;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;
    private readonly IUserService _userService;

    public PaymentRequestService(
        ILogger<PaymentRequestService> logger,
        IOptions<CosmosDbOptions> options,
        CosmosClient client,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory,
        IUserService userService
        )
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.PaymentRequestsContainerId);
        _userService = userService;
    }

    public async Task<Result> CreatePaymentRequest(PaymentRequest request)
    {

        if (request.CreditorUserId == request.DebtorUserId)
        {
            _logger.LogWarning("Creditor user ID {creditorUserId} cannot be same as debtor user ID {debtorUserId}", request.CreditorUserId, request.DebtorUserId);
            return Result.Failure(PaymentRequestErrors.CreditorAndDebtorIdentical(request.CreditorUserId.ToString(), request.DebtorUserId.ToString()));
        }

        var user = await _userService.GetUserById(request.DebtorUserId.ToString());
        var toBeBalance = user.Balance - request.Amount;
        _logger.LogInformation("Current user balance {currentBalance}; to-be balance {tobeBalance}", user.Balance, toBeBalance);
        if (toBeBalance < 0)
        {
            var description = $"Insuffient fund to complete payment request for debtor {user.Id}. Current balance {user.Balance.ToString("C3", CultureInfo.CreateSpecificCulture("en-GB"))}; to-be balance {toBeBalance.ToString("C3", CultureInfo.CreateSpecificCulture("en-GB"))}";
            _logger.LogInformation(description);
            return Result.Failure(PaymentRequestErrors.DebtorInsufficientFund(request.DebtorUserId.ToString()));
        }

        _logger.LogInformation("Creating payment request from {DebtorUserId} to {CreditorUserId}", request.DebtorUserId, request.CreditorUserId);
        await _container.CreateItemAsync(request, new PartitionKey(request.Id.ToString()));
        var _sender = _serviceBusSenderFactory.CreateClient("queue-sender");
        string messageBody = JsonConvert.SerializeObject(request);
        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(messageBody));
        await _sender.SendMessageAsync(message);
        return Result.Success();
    }
}
