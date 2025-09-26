using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using property_price_api.Data;
using property_price_api.Models;
using property_price_api.Profiles;
using property_price_api.Services;

namespace unit_tests;

[TestFixture]
[Category("Integration")]
public class PropertyServiceTests
{
    private ServiceProvider _serviceProvider;
    private IOptions<PropertyPriceApiDatabaseSettings> _config;

    [OneTimeSetUp]
    public void GlobalPrepare()
    {
        var configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", false)
           .Build();

        _config = Options.Create(configuration.GetSection("PropertyPriceApiDatabase").Get<PropertyPriceApiDatabaseSettings>());
    }

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        services.AddSingleton(serviceProvider =>
        {
            return new MongoDbContext(
        Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING") ??
        _config.Value.DatabaseName,
        _config.Value.DatabaseName);
        });

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new UserProfile());
            mc.AddProfile(new PropertyProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
        services.AddHttpContextAccessor();
        services.AddSingleton<IPropertyService, PropertyService>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task GetProperties()
    {
        var propertyService = _serviceProvider.GetService<IPropertyService>();
        var startDate = new DateTime(2020, 04, 01);
        var endDate = new DateTime(2025, 06, 01);
        var properties = await propertyService.GetProperties(startDate, endDate);
        Console.WriteLine("Retrieved {0} properties from database.", properties.Count);
        Assert.That(properties.Count > 0);
    }

    [Test]
    public void ReadCSV()
    {
        FileStream stream = File.OpenRead("resources/example-properties.csv");
        var propertyService = _serviceProvider.GetService<IPropertyService>();
        var createPropertyRequests = propertyService.ReadCSV<CreatePropertyRequest>(stream);

        // Assert
        Assert.That(createPropertyRequests.Count(), Is.EqualTo(3));
    }
}