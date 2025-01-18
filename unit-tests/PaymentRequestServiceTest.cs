using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

namespace unit_tests;

[TestFixture]
[Category("Integration")]
public class PaymentRequestServiceTest
{
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        services.Configure<CosmosDbOptions>(options =>
        {
            options.DatabaseId = "transactionsdb";
            options.PaymentRequestsContainerId = "payment-requests";
            options.UsersContainerId = "users";
        });
        services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<CosmosDbOptions>>().Value;
    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOS_DB_CONNECTION_STRING") ?? settings.ConnectionString);
});
        services.AddSingleton<IPaymentRequestService, PaymentRequestService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient("foo").WithName("main");
            clientBuilder.AddServiceBusClientWithNamespace("foo").WithName("main");
            clientBuilder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                        provider
                        .GetService<IAzureClientFactory<ServiceBusClient>>()
                        .CreateClient("main")
                        .CreateSender("foo")
                    ).WithName("topic-sender");
        });

        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task CreatePaymentRequestInvalidUserIdShould()
    {
        var paymentRequestService = _serviceProvider.GetService<IPaymentRequestService>();
        var debtorUserId = Guid.NewGuid();
        var creditorUserId = Guid.NewGuid();
        var request = new PaymentRequest { DebtorUserId = debtorUserId, CreditorUserId = creditorUserId, Amount = 10 };
        var res = await paymentRequestService.CreatePaymentRequest(request);
        Assert.Multiple(() =>
        {
            Assert.That(res.IsFailure, Is.True);
            Assert.That(res.Error.ToString(), Does.Contain("PaymentRequestErrors.InvalidUserId"));
        });
    }
}
