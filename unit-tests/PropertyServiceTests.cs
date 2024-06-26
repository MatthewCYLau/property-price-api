using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using property_price_api.Data;
using property_price_api.Profiles;
using property_price_api.Services;

namespace unit_tests;

[Ignore("Test to be ran locally")]
[TestFixture]
[Category("Integration")]
public class PropertyServiceTests
{
    private ServiceProvider _serviceProvider;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        services.AddSingleton(_ => new MongoDbContext(
            "foo",
            "bar"));
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
        var startDate = new DateTime(2024, 04, 01);
        var endDate = new DateTime(2024, 06, 01);
        var properties = await propertyService.GetProperties(startDate, endDate);
        Console.WriteLine("Retrieved {0} properties from database.", properties.Count);
        Assert.That(properties.Count > 0);
    }
}