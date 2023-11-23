using Microsoft.Extensions.Options;
using property_price_api.Models;
using property_price_api.Data;
using property_price_ingest;
using property_price_ingest.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<PropertyPriceApiDatabaseSettings>(
    builder.Configuration.GetSection("PropertyPriceApiDatabase"));
builder.Services.AddSingleton(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<PropertyPriceApiDatabaseSettings>>().Value;
    return new MongoDbContext(
        Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING") ??
        settings.ConnectionString,
        settings.DatabaseName);
});

builder.Services.AddHostedService<IngestWorker>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

IHost host = builder.Build();
host.Run();
