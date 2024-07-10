using AutoFixture;
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
    private IFixture _fixture;
    private readonly string ListingURL = "https://www.rightmove.co.uk/properties/141178922#/?channel=RES_BUY";
    private readonly string Address = "Cardinal Close, Worcester Park";
    private readonly int AskingPrice = 555_000;

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
        _fixture = new Fixture();
        var mockPropertyService = new Mock<IPropertyService>();

        // given
        var startDate = new DateTime(2024, 01, 30);
        var endDate = new DateTime(2024, 05, 30);
        var testPropertyDtos1 = _fixture.CreateMany<PropertyDto>(5).ToList();
        List<PropertyDto> testPropertyDtos2 = new List<PropertyDto>
        {
            new()
            {
                ListingUrl = ListingURL,
                Address = Address,
                AskingPrice = AskingPrice
            }
        };
        var testPropertyDtos = testPropertyDtos2.Concat(testPropertyDtos1).ToList();

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
        Assert.That(resultPropertyDtos.Count, Is.EqualTo(6));
        Assert.That(resultPropertyDtos[0].AskingPrice, Is.EqualTo(555_000));
    }

    [Test]
    public async Task GetPropertyByIdShould()
    {
        ILogger<PropertiesController> logger = _serviceProvider.GetRequiredService<ILogger<PropertiesController>>();
        var mockPropertyService = new Mock<IPropertyService>();

        // given
        var testPropertyDto = new PropertyDto
        {
            ListingUrl = ListingURL,
            Address = Address,
            AskingPrice = AskingPrice
        };

        mockPropertyService.Setup(x => x.GetPropertyById("1")).Returns(Task.FromResult(testPropertyDto));
        var propertiesController = new PropertiesController(logger, mockPropertyService.Object);

        // when
        var propertyResult = await propertiesController.GetPropertyById("1");
        OkObjectResult? okResult = propertyResult.Result as OkObjectResult;

        // Assert
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(testPropertyDto));

        var resultPropertyDto = (PropertyDto)okResult.Value;
        Assert.That(resultPropertyDto.AskingPrice, Is.EqualTo(555_000));
    }

    [Test]
    public async Task GetPropertyByInvalidIdShouldNotFound()
    {
        ILogger<PropertiesController> logger = _serviceProvider.GetRequiredService<ILogger<PropertiesController>>();
        var mockPropertyService = new Mock<IPropertyService>();

        mockPropertyService.Setup(x => x.GetPropertyById("foo")).Returns(Task.FromResult((PropertyDto)null));
        var propertiesController = new PropertiesController(logger, mockPropertyService.Object);

        // when
        var propertyResult = await propertiesController.GetPropertyById("foo");
        NotFoundResult? notFoundResult = propertyResult.Result as NotFoundResult;

        // Assert
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
    }
}