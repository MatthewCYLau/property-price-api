namespace property_price_cosmos_db.Services;

public class PaymentRequestWorker(ILogger<PaymentRequestWorker> logger
) : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<PaymentRequestWorker> _logger = logger;

    public async ValueTask DisposeAsync()
    {

    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{worker} is running...", nameof(PaymentRequestWorker));

    }
}
