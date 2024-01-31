using Microsoft.AspNetCore.Mvc;
using property_price_purchase_service.Models;
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
    
    [HttpPost]
    public IActionResult CreateOrder(OrderRequest request)
    {
        _ordersService.CreateOrder(request);
        return Ok(new { message = "Order created" });
    }
    
    [HttpGet("{id}")]
    public ActionResult<Order> GetOrderById(int id)
    {
        var order =_ordersService.GetOrderById(id);
        return Ok(order);
    }
    
    [HttpPatch("{id}")]
    public ActionResult<Order> UpdateOrderById(int id, OrderRequest request)
    {
        var order =_ordersService.UpdateOrderById(id, request);
        return Ok(order);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteOrderById(int id)
    {
        _ordersService.DeleteOrderById(id);
        return Ok(new { message = "Order deleted" });
    }
}