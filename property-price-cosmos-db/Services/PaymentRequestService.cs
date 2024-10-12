using System.Globalization;
using Microsoft.Azure.Cosmos;
using Azure.Storage.Blobs;
using CsvHelper;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using property_price_cosmos_db.Models;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using property_price_common;

namespace property_price_cosmos_db.Services;

public class PaymentRequestService : IPaymentRequestService
{
    private readonly CosmosClient _client;
    private readonly CosmosDbOptions _options;
    private Container _container;
    private readonly ILogger _logger;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;

    public PaymentRequestService(
        IOptions<CosmosDbOptions> options,
        CosmosClient client,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory
        )
    {
        _client = client;
        _options = options.Value;
        _serviceBusSenderFactory = serviceBusSenderFactory;
        _container = _client.GetContainer(_options.DatabaseId, _options.PaymentRequestsContainerId);
    }

    public async Task<Result> CreatePaymentRequest(PaymentRequest request)
    {
        await _container.CreateItemAsync(request, new PartitionKey(request.Id.ToString()));
        var _sender = _serviceBusSenderFactory.CreateClient("queue-sender");
        string messageBody = JsonConvert.SerializeObject(request);
        ServiceBusMessage message = new(Encoding.UTF8.GetBytes(messageBody));
        await _sender.SendMessageAsync(message);
        return Result.Success();
    }
}
