﻿using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api")]
    public class JobsController : ControllerBase
    {

        private readonly ICloudPubSubService _cloudPubSubService;
        private readonly IIngestJobService _ingestJobService;

        public JobsController(ICloudPubSubService cloudPubSubService, IIngestJobService ingestJobService)
        {
            _cloudPubSubService = cloudPubSubService;
            _ingestJobService = ingestJobService;
        }

        [Authorize]
        [HttpPost("jobs")]
        public async Task<ActionResult<CreateJobResponse>> CreateIngestJob(CreateJobRequest request)
        {

            if (!StringHelpers.IsValidateUkPostcode(request.Postcode))
            {
                return BadRequest(new { message = "Invalid UK postcode!" });
            }

            var jobId = await _ingestJobService.CreateIngestJob(request.Postcode);
            var messageId = await _cloudPubSubService.PublishMessagesAsync(new CloudPubSubMessage(jobId, request.Postcode));
            var res = new CreateJobResponse(messageId, jobId);

            return CreatedAtAction(nameof(CreateIngestJob), res);

        }

        [Authorize]
        [HttpGet("jobs")]
        public async Task<ActionResult<List<IngestJob>>> GetIngestJobs([FromQuery] bool complete = false, string postcode = "")
        {
            return await _ingestJobService.GetIngestJobs(complete, postcode);
        }


        [HttpGet("jobs/{id:length(24)}")]
        public async Task<ActionResult<IngestJob>> GetIngestJobById(string id)
        {
            var ingestJob = await _ingestJobService.GetIngestJobById(id);

            if (ingestJob is null)
            {
                return NotFound();
            }

            return ingestJob;
        }

        [Authorize]
        [HttpGet]
        [Route("ingested-price")]
        public async Task<ActionResult<IngestedPriceResponse>> GetIngestedTransactionPrice([FromQuery] string postcode)
        {
            var jobs = await _ingestJobService.GetIngestJobs(true, postcode);
            if (!jobs.Any())
            {
                return NotFound();
            }
            var meanIngestedPrice = jobs.Select(x => x.TransactionPrice).Average();
            var res = new IngestedPriceResponse(postcode, (int)meanIngestedPrice);
            return Ok(res);

        }

        [Authorize]
        [HttpDelete("jobs/{id:length(24)}")]
        public async Task<IActionResult> DeleteIngestJobById(string id)
        {
            var job = await _ingestJobService.GetIngestJobById(id);

            if (job is null)
            {
                return NotFound();
            }

            await _ingestJobService.DeleteIngestJobById(id);

            return NoContent();

        }
    }
}

