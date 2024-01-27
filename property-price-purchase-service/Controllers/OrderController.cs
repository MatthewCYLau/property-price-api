using Microsoft.AspNetCore.Mvc;

namespace property_price_purchase_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public string Ping() => "pong!";
}