using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace property_price_cosmos_db.Services;

public class PaymentRequestService : IPaymentRequestService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private readonly Container _container;
    private readonly ILogger _logger;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;

    public PaymentRequestService(
        ILogger<PaymentRequestService> logger,
        IOptions<CosmosDbOptions> options,
        CosmosClient client,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory
        )
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.PaymentRequestsContainerId);
    }

    public async Task<Result> CreatePaymentRequest(PaymentRequest request)
    {
        _logger.LogInformation("Creating payment request from {DebtorUserId} to {CreditorUserId}", request.DebtorUserId, request.CreditorUserId);
        await _container.CreateItemAsync(request, new PartitionKey(request.Id.ToString()));
        var _sender = _serviceBusSenderFactory.CreateClient("queue-sender");
        string messageBody = JsonConvert.SerializeObject(request);
        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(messageBody));
        await _sender.SendMessageAsync(message);
        return Result.Success();
    }
}
