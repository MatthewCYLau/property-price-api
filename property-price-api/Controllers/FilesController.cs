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

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
            return Ok(new { file.FileName, filePath });
        }
    }
}

