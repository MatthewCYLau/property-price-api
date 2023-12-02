using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Services;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IGoogleCloudStorageService _googleCloudStorageService;

        public FilesController(IGoogleCloudStorageService googleCloudStorageService)
        {
            _googleCloudStorageService = googleCloudStorageService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            var url = await _googleCloudStorageService.UploadFileAsync(file);
            return Ok(new { file.FileName, url });
        }
    }
}

