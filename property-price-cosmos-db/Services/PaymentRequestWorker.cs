using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;
using property_price_cosmos_db.Models;
using Newtonsoft.Json;
using System.Text;

namespace property_price_cosmos_db.Services;

public class PaymentRequestWorker(
    ILogger<PaymentRequestWorker> logger,
    ITransactionService transactionService,
    IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
    IConfiguration configuration
) : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<PaymentRequestWorker> _logger = logger;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory = serviceBusClientFactory;
    private readonly IConfiguration _configuration = configuration;

    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;


    public async ValueTask DisposeAsync()
    {

    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(PaymentRequestWorker));
        var _client = _serviceBusClientFactory.CreateClient("main");
        _processor = _client.CreateProcessor(_configuration.GetValue<string>(
                "Azure:ServiceBus:Queue"), new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        PaymentRequest request = JsonConvert.DeserializeObject<PaymentRequest>(Encoding.UTF8.GetString(args.Message.Body));
        _logger.LogInformation("Received payment request from {DebtorUserId} to {CreditorUserId}", request.DebtorUserId, request.CreditorUserId);
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception.ToString());
    }
}
