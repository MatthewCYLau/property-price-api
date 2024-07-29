using Moq;
using property_price_api.Controllers;
using property_price_api.Models;
using property_price_api.Services;

namespace unit_tests;

public class AnalysisJobsControllerTests
{
    private const string testPostcode = "SE21 7LG";

    [Test]
    public async Task GetIngestJobsShould()
    {
        var testIngestJob = new IngestJob(testPostcode);
        var mockIngestJobsService = new Mock<IIngestJobService>();
        var mockCloudPubSubService = new Mock<ICloudPubSubService>();

        mockIngestJobsService.Setup(x => x.GetIngestJobs(false, testPostcode, 1, 5)).Returns(Task.FromResult(
            new IngestJobsResponse(new PaginationMetadata(1, 1, 1), [testIngestJob])
        ));
        var jobsController = new JobsController(mockCloudPubSubService.Object, mockIngestJobsService.Object);
        var jobs = await jobsController.GetIngestJobs(false, testPostcode, 1, 5);
        Assert.That(jobs.Value.IngestJobs, Has.Count.EqualTo(1));
    }
}
