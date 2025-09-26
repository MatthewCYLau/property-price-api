using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FilesController(
        IGoogleCloudStorageService googleCloudStorageService,
        ILogger<FilesController> logger,
        IPropertyService propertyService) : ControllerBase
    {
        private readonly IGoogleCloudStorageService _googleCloudStorageService = googleCloudStorageService;
        private readonly IPropertyService _propertyService = propertyService;
        private readonly ILogger _logger = logger;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {

            using (var stream = file.OpenReadStream())
            {
                var createPropertyRequests = _propertyService.ReadCSV<CreatePropertyRequest>(stream);
                foreach (var req in createPropertyRequests)
                {
                    _logger.LogInformation("Reading property for address {address}", req.Address);
                }
            }

            var url = await _googleCloudStorageService.UploadFileAsync(file);
            return Ok(new { file.FileName, url });
        }
    }
}

