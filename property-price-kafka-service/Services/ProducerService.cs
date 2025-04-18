using Confluent.Kafka;

namespace property_price_kafka_service.Services;

public class ProducerService
{
    private readonly IConfiguration _configuration;

    private readonly IProducer<Null, string> _producer;

    public ProducerService(IConfiguration configuration)
    {
        _configuration = configuration;

        var producerconfig = new ProducerConfig
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_HOST") ?? _configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerconfig).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        var kafkamessage = new Message<Null, string> { Value = message, };

        await _producer.ProduceAsync(topic, kafkamessage);
    }
}
