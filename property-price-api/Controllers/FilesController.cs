using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;

namespace property_price_api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {

            var filePath = Path.GetTempFileName();
            var url = "";

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);

                var client = StorageClient.Create();
                var obj1 = client.UploadObject("property-price-engine-assets", file.FileName, file.ContentType, stream);
                url = obj1.SelfLink;
            }
            return Ok(new { file.FileName, url });
        }
    }
}

