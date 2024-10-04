using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;
using property_price_cosmos_db.Models;
using Newtonsoft.Json;
using System.Text;

namespace property_price_cosmos_db.Services;

public class TrasantionWorker : BackgroundService, IAsyncDisposable
{

    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private readonly ILogger<TrasantionWorker> _logger;
    private readonly IAzureClientFactory<ServiceBusClient> _serviceBusClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;


    public TrasantionWorker(
        ILogger<TrasantionWorker> logger,
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IConfiguration configuration,
        IUserService userService
        )
    {
        _logger = logger;
        _serviceBusClientFactory = serviceBusClientFactory;
        _configuration = configuration;
        _userService = userService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(TrasantionWorker));
        var _client = _serviceBusClientFactory.CreateClient("main");
        _processor = _client.CreateProcessor(_configuration.GetValue<string>(
                "Azure:ServiceBus:Topic"), _configuration.GetValue<string>(
                "Azure:ServiceBus:Subscription"), new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{worker} is stopping...", nameof(TrasantionWorker));
        // await _processor.StopProcessingAsync();
        // await base.StopAsync(cancellationToken);
    }


    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        Transaction transaction = JsonConvert.DeserializeObject<Transaction>(Encoding.UTF8.GetString(args.Message.Body));
        _logger.LogInformation("Received message from Service Bus for trasaction {id}", transaction.Id);
        var amount = transaction.Amount;
        switch (transaction.TransactionType)
        {
            case TransactionType.Debit:
                {
                    _logger.LogInformation("Processing debit transaction {amount}", transaction.Amount);
                    amount *= -1;
                    break;
                }
            case TransactionType.Credit:
                {
                    _logger.LogInformation("Processing credit transaction {amount}", transaction.Amount);
                    break;
                }
            default: break;
        }
        var res = await _userService.UpdateUserBalanceById(transaction.UserId.ToString(), amount);
        _logger.LogInformation("Updated user {id}. Updated balance {balance}", res.Id, res.Balance);
        await args.CompleteMessageAsync(args.Message);
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception.ToString());
    }

    public async ValueTask DisposeAsync()
    {
        await _processor.DisposeAsync();
        // await _client.DisposeAsync();
    }
}
