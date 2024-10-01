using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Microsoft.Extensions.Options;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CosmosDbOptions>(
    builder.Configuration.GetSection(CosmosDbOptions.CosmosDbSettingsName));

builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? settings.ConnectionString);
});
builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddHostedService<TrasantionWorker>();

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
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "development" ? new DefaultAzureCredential() : credential
    );

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetSection("Azure:Storage")).WithName("main");
    clientBuilder.AddServiceBusClientWithNamespace("sbns-gitlab-azure-terraform-production.servicebus.windows.net");
    clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                provider
                .GetService<ServiceBusClient>()
                .CreateSender("sbt-aks-storage-request")
            ).WithName("sbt-aks-storage-request-sender");
    // clientBuilder.AddClient<ServiceBusReceiver, ServiceBusClientOptions>((_, _, provider) =>
    //               provider
    //               .GetService<ServiceBusClient>()
    //               .CreateReceiver("sbt-aks-storage-request", "aks-storage-request")
    //           ).WithName("queueNamde");
    clientBuilder.UseCredential(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "development" ? new DefaultAzureCredential() : credential);
});

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

app.MapGet("/ping", () => "pong!");

app.Run();
