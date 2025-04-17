using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using property_price_cosmos_db.Helper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CosmosDbOptions>(
    builder.Configuration.GetSection(CosmosDbOptions.CosmosDbSettingsName));
builder.Services.Configure<ManagedIdentityOptions>(
    builder.Configuration.GetSection(ManagedIdentityOptions.ManagedIdentitySettingsName));

builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? settings.ConnectionString);
});
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<ICustomInitService, CustomInitService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPaymentRequestService, PaymentRequestService>();
builder.Services.AddHostedService<TrasantionWorker>();
builder.Services.AddHostedService<PaymentRequestWorker>();
// builder.Services.AddHostedService<EventProcessorService>();

var clientId = builder.Configuration
    .GetSection(ManagedIdentityOptions.ManagedIdentitySettingsName)
    .Get<ManagedIdentityOptions>().CliendId;

var credential = new DefaultAzureCredential(
    new DefaultAzureCredentialOptions
    {
        ManagedIdentityClientId = clientId
    });

builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "development" ? new AzureCliCredential() : credential
    );

builder.Services.AddAzureClients(clientBuilder =>
{
    var topic = builder.Configuration.GetValue<string>("Azure:ServiceBus:Topic");
    var queue = builder.Configuration.GetValue<string>("Azure:ServiceBus:Queue");
    var eventHubsNamespace = builder.Configuration.GetValue<string>("Azure:EventHubs:Namespace");
    var eventHubName = builder.Configuration.GetValue<string>("Azure:EventHubs:EventHubName");
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Azure:Storage")).WithName("main");
    clientBuilder.AddServiceBusClientWithNamespace($"{builder.Configuration["Azure:ServiceBus:Name"]}.servicebus.windows.net").WithName("main");
    clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                provider
                .GetService<IAzureClientFactory<ServiceBusClient>>()
                .CreateClient("main")
                .CreateSender(topic)
            ).WithName("topic-sender");
    clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                 provider
                 .GetService<IAzureClientFactory<ServiceBusClient>>()
                 .CreateClient("main")
                 .CreateSender(queue)
             ).WithName("queue-sender").ConfigureOptions(options =>
          {
              options.RetryOptions.Delay = TimeSpan.FromMilliseconds(50);
              options.RetryOptions.MaxDelay = TimeSpan.FromSeconds(5);
              options.RetryOptions.MaxRetries = 3;
          });
    // clientBuilder.AddClient<ServiceBusReceiver, ServiceBusClientOptions>((_, _, provider) =>
    //               provider
    //               .GetService<ServiceBusClient>()
    //               .CreateReceiver("sbt-aks-storage-request", "aks-storage-request")
    //           ).WithName("sbt-aks-storage-request-receiver");
    // clientBuilder.AddEventHubProducerClientWithNamespace(eventHubsNamespace, "example").WithName("event-hub-producer");
    clientBuilder.UseCredential(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "development" ? new AzureCliCredential() : credential);
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CosmosDB Service",
    });
});

builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheckService>("main-health-check");

// builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration["RedisSettings:Host"]));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.GetRequiredService<ITransactionService>().CreateSeedTransactions();
app.Services.GetRequiredService<ICustomInitService>().GetAssembly();

app.MapGet("/ping", () => "pong!");
app.MapGet("/async", async () => await MathsHelper.GetRandomNumberInclusiveAsync(1, 100));
app.MapHealthChecks("/healthz");
app.Run();
