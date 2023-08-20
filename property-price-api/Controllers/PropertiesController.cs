using Microsoft.AspNetCore.Mvc;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController: ControllerBase
    {

        private readonly PropertyService _propertyService;

        public PropertiesController(PropertyService propertyService) =>
            _propertyService = propertyService;

        [HttpGet]
        public async Task<List<PropertyDto>> Get() =>
            await _propertyService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Property>> Get(string id)
        {
            var property = await _propertyService.GetAsync(id);

            if (property is null)
            {
                return NotFound();
            }

            return property;
        }


        [HttpPost]
        public async Task<IActionResult> Post(Property property)
        {
            await _propertyService.CreateAsync(property);

            return CreatedAtAction(nameof(Get), new { id = property.Id }, property);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Property updatedProperty)
        {
            var property = await _propertyService.GetAsync(id);

            if (property is null)
            {
                return NotFound();
            }

            updatedProperty.Id = property.Id;

            await _propertyService.UpdateAsync(id, updatedProperty);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var property = await _propertyService.GetAsync(id);

            if (property is null)
            {
                return NotFound();
            }

            await _propertyService.RemoveAsync(id);

            return NoContent();
        }
    }
}

