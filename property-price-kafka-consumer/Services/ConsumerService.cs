
using Confluent.Kafka;

namespace property_price_kafka_consumer.Services;


public class ConsumerService : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;

    private readonly ILogger<ConsumerService> _logger;
    private readonly IConfiguration _configuration;

    public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
    {
        _logger = logger;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_HOST") ?? configuration["Kafka:BootstrapServers"],
            GroupId = "InventoryConsumerGroup",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _configuration = configuration;
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_configuration["Kafka:Topic"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            ProcessKafkaMessage(stoppingToken);

            Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _consumer.Close();
    }

    public void ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        try
        {
            var consumeResult = _consumer.Consume(stoppingToken);

            var message = consumeResult.Message.Value;

            _logger.LogInformation($"Received inventory update: {message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing Kafka message: {ex.Message}");
        }
    }

}
