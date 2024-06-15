using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using property_price_api.Controllers;
using property_price_api.Services;

namespace unit_tests;

public class PropertiesControllerTests
{
    
    private ServiceProvider _serviceProvider;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [Test]
    public async Task GetPropertiesShouldReturnExceptionInvalidDates()
    {
        ILogger<PropertiesController> logger = _serviceProvider.GetRequiredService<ILogger<PropertiesController>>();
        var mockPropertyService = new Mock<IPropertyService>();
        mockPropertyService.Setup(x => x.GetProperties(new DateTime(), new DateTime?()));
        var propertiesController = new PropertiesController(logger, mockPropertyService.Object);
        var startDate = new DateTime(2024, 05, 30);
        var endDate = new DateTime(2024, 01, 30);
        var res = await propertiesController.Get(startDate, endDate);
        Assert.IsInstanceOf<BadRequestObjectResult>(res.Result);
    }
}