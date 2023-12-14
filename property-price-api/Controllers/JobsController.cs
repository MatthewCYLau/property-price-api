﻿using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
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
        [HttpPost]
        public async Task<ActionResult<CreateJobResponse>> CreateIngestJob(CreateJobRequest request)
        {

            var jobId = await _ingestJobService.CreateIngestJob(request.Postcode);
            var messageId = await _cloudPubSubService.PublishMessagesAsync(new CloudPubSubMessage(jobId, request.Postcode));
            var res = new CreateJobResponse(messageId, jobId);

            return CreatedAtAction(nameof(CreateIngestJob), res);

        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<IngestJob>> GetIngestJobById(string id)
        {
            var ingestJob = await _ingestJobService.GetIngestJobById(id);

            if (ingestJob is null)
            {
                return NotFound();
            }

            return ingestJob;
        }
    }
}

