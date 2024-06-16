using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using property_price_api.Controllers;
using property_price_api.Models;
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
    
    [Test]
    public async Task GetPropertiesShould()
    {
        ILogger<PropertiesController> logger = _serviceProvider.GetRequiredService<ILogger<PropertiesController>>();
        var mockPropertyService = new Mock<IPropertyService>();
        
        // given
        var startDate = new DateTime(2024, 01, 30);
        var endDate = new DateTime(2024, 05, 30);
        List<PropertyDto> testPropertyDtos = new List<PropertyDto>
        {
            new()
            {
                ListingUrl = "https://www.rightmove.co.uk/properties/141178922#/?channel=RES_BUY",
                Address = "Cardinal Close, Worcester Park",
                AskingPrice = 555_000
            }
        };
        
        mockPropertyService.Setup(x => x.GetProperties(startDate, endDate)).Returns(Task.FromResult(testPropertyDtos));
        var propertiesController = new PropertiesController(logger, mockPropertyService.Object);
        
        // when
        var propertiesResult = await propertiesController.Get(startDate, endDate);
        OkObjectResult? okResult = propertiesResult.Result as OkObjectResult;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(testPropertyDtos));
        
        List<PropertyDto> resultPropertyDtos = (List<PropertyDto>)okResult.Value;
        Assert.That(resultPropertyDtos[0].AskingPrice, Is.EqualTo(555_000));
    }
}