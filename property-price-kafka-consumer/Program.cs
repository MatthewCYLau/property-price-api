using property_price_kafka_consumer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConsumerService>();
var app = builder.Build();
app.Run();