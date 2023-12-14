using Microsoft.Extensions.Options;
using property_price_api.Data;
using property_price_ingest;
using property_price_ingest.Services;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using property_price_api.Models;
using Google.Cloud.PubSub.V1;
using property_price_api.Services;

var builder = WebApplication.CreateBuilder(args);

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
SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(
    builder.Configuration.GetValue<string>("CloudPubSub:GcpProjectId"),
    builder.Configuration.GetValue<string>("CloudPubSub:SubscriptionName"));
builder.Services.AddSubscriberClient(subscriptionName);
builder.Services.AddSingleton<IIngestJobService, IngestJobService>();

// Configure HTTP client
builder.Services.AddHttpClient(HttpClientConstants.jsonPlaceholderHttpClientName, httpClient =>
{
    httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
    httpClient.DefaultRequestHeaders.Add(
        HeaderNames.Accept, "application/json");
});

var app = builder.Build();
app.MapGet("/", () => "up!");

app.Run();