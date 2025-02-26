
using property_price_kafka_service.Models;
using property_price_kafka_service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
namespace property_price_kafka_service;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly ProducerService _producerService;
    private readonly IConfiguration _configuration;

    public InventoryController(ProducerService producerService, IConfiguration configuration
)
    {
        _producerService = producerService;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateInventory([FromBody] InventoryUpdateRequest request)
    {
        var message = JsonSerializer.Serialize(request);

        await _producerService.ProduceAsync(_configuration["Kafka:Topic"], message);

        return Ok("Inventory Updated Successfully...");
    }
}
