using System;
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
        public async Task<List<Property>> Get() =>
            await _propertyService.GetAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Property property)
        {
            await _propertyService.CreateAsync(property);

            return CreatedAtAction(nameof(Get), new { id = property.Id }, property);
        }
    }
}

