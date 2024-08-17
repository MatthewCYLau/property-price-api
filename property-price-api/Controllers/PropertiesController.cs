using System.Text.Json;
using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;
using StackExchange.Redis;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {

        private readonly IPropertyService _propertyService;
        private readonly ILogger _logger;
        // private readonly IDatabase _redis;

        public PropertiesController(
            ILogger<PropertiesController> logger,
            IPropertyService propertyService
            // IConnectionMultiplexer muxer
            )
        {
            _propertyService = propertyService;
            _logger = logger;
            // _redis = muxer.GetDatabase();
        }

        [HttpGet]
        public async Task<ActionResult<List<PropertyDto>>> Get(DateTime? startDate, DateTime? endDate)
        {
            _logger.LogInformation("Get properties from {startDate} and {endDate}", startDate, endDate);
            if (endDate < startDate)
            {
                return BadRequest(new { message = "End date must be greater than start date" });
            }
            var res = await _propertyService.GetProperties(startDate, endDate);
            return Ok(res);
        }


        [HttpGet("{id:length(24)}/price-analysis")]
        public async Task<ActionResult<PriceAnalysisResponse>> GetPropertyPriceAnalysisById(string? id)
        {
            var res = await _propertyService.GeneratePriceAnalysisByPropertyId(id);
            return Ok(res);
        }


        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(string? id)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            return Ok(property);

            // string json = await _redis.StringGetAsync(id);
            // if (string.IsNullOrEmpty(json))
            // {
            //     var property = await _propertyService.GetPropertyById(id);
            //     string jsonString = JsonSerializer.Serialize(property);
            //     var setTask = _redis.StringSetAsync(id, jsonString);
            //     var expireTask = _redis.KeyExpireAsync(id, TimeSpan.FromSeconds(3600));
            //     await Task.WhenAll(setTask, expireTask);
            //     return Ok(property);
            // }
            // var propertyFromRedis =
            //     JsonSerializer.Deserialize<PropertyDto>(json);
            // return Ok(propertyFromRedis);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreatePropertyResponse>> CreateProperty(CreatePropertyRequest createPropertyRequest)
        {
            var _res = await _propertyService.CreateProperty(createPropertyRequest);

            return CreatedAtAction(nameof(Get), new { id = _res.Id }, _res);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdatePropertyById(string? id, UpdatePropertyRequest updatePropertyRequest)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            if (!await _propertyService.UpdatePropertyById(id, updatePropertyRequest))
            {
                return BadRequest(new { message = "Update property went wrong!" });
            }

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePropertyById(string? id)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            await _propertyService.DeletePropertyById(id);

            return NoContent();
        }

        [Authorize]
        [Produces("text/csv")]
        [HttpPost("export-csv")]
        public async Task<FileResult> ExportCsv(DateTime? startDate, DateTime? endDate)
        {
            using var memoryStream = new MemoryStream();
            _logger.LogInformation("Get properties from {startDate} and {endDate}", startDate, endDate);
            if (endDate < startDate)
            {
                throw new CustomException("End date must be greater than start date");
            }
            var propertyDtos = await _propertyService.GetProperties(startDate, endDate);
            await using (var streamWriter = new StreamWriter(memoryStream))
            await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                await csvWriter.WriteRecordsAsync(propertyDtos);
            }

            return File(memoryStream.ToArray(), "text/csv", $"properties-export-{DateTime.Now:yyyy-MM-dd}.csv");
        }

        [Authorize]
        [HttpPost("import-from-csv")]
        public IActionResult ImportPropertiesFromCsv([FromForm] IFormFileCollection file)
        {
            var createPropertyRequests = _propertyService.ReadCSV<CreatePropertyRequest>(file[0].OpenReadStream());
            foreach (var request in createPropertyRequests)
            {
                _propertyService.CreateProperty(request);
            }
            return Ok();
        }

        [Authorize]
        [HttpPost("import-from-cloud-storage")]
        public IActionResult ImportPropertiesFromCloudStorage(ImportPropertiesFromCloudStorageRequest request)
        {
            _logger.LogInformation("Import properties from Cloud Storage object {url}", request.ObjectUrl);
            return Ok();
        }
    }
}

