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

        public JobsController(ICloudPubSubService cloudPubSubService)
        {
            _cloudPubSubService = cloudPubSubService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreateJobResponse>> CreateIngestJob(CloudPubSubMessage message)
        {
            
            var messageId = await _cloudPubSubService.PublishMessagesAsync(message);
            var res = new CreateJobResponse(messageId);

            return CreatedAtAction(nameof(CreateIngestJob), res);

        }
    }
}
