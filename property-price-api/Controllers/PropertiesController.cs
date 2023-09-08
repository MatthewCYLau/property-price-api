using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController: ControllerBase
    {

        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService) =>
            _propertyService = propertyService;

        [HttpGet]
        public async Task<List<PropertyDto>> Get() =>
            await _propertyService.GetProperties();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(string id)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            return property;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CreatePropertyResponse>> CreateProperty(CreatePropertyRequest createPropertyRequest)
        {
            var _res = await _propertyService.CreateProperty(createPropertyRequest);

            return CreatedAtAction(nameof(Get), new { id = _res.Id }, _res);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdatePropertyById(string id, Property updatedProperty)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            updatedProperty.Id = property.Id;

            await _propertyService.UpdatePropertyById(id, updatedProperty);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeletePropertyById(string id)
        {
            var property = await _propertyService.GetPropertyById(id);

            if (property is null)
            {
                return NotFound();
            }

            await _propertyService.DeletePropertyById(id);

            return NoContent();
        }
    }
}

