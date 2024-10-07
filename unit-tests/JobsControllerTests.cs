using Microsoft.AspNetCore.Mvc;
using Moq;
using property_price_api.Controllers;
using property_price_api.Services;
using property_price_api.Models;

namespace unit_tests;

public class JobsControllerTests
{

    [Test]
    public async Task CreateIngestJobShouldReturnExceptionInvalidPostcode()
    {
        var mockPropertyService = new Mock<IIngestJobService>();
        var mockCloudPubSubService = new Mock<ICloudPubSubService>();
        var jobsController = new JobsController(mockCloudPubSubService.Object, mockPropertyService.Object);
        var res = await jobsController.CreateIngestJob(new CreateJobRequest() { Postcode = "sadfds" });
        Assert.That(res.Result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
