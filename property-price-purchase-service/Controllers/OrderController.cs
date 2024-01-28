using Microsoft.AspNetCore.Mvc;
using property_price_purchase_service.Services;

namespace property_price_purchase_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;

    public OrdersController(IOrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        var orders =_ordersService.GetOrders();
        return Ok(orders);
    }
}