using Microsoft.AspNetCore.Mvc;
using property_price_purchase_service.Models;
using property_price_purchase_service.Services;

namespace property_price_purchase_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController: ControllerBase
{
    private readonly IProductsService _productsService;

    public ProductController(IProductsService productsService)
    {
        _productsService = productsService;
    }
    
    [HttpGet]
    public IActionResult GetProducts()
    {
        var products =_productsService.GetProducts();
        return Ok(products);
    }
    
    [HttpPost]
    public IActionResult CreateProduct(ProductRequest request)
    {
        _productsService.CreateProduct(request);
        return Ok(new { message = "Product created" });
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteProductById(int id)
    {
        _productsService.DeleteProductById(id);
        return Ok(new { message = "Product deleted" });
    }
    
    [HttpPut("{id}")]
    public ActionResult<Product> UpdateProductById(int id, ProductRequest request)
    {
        var order =_productsService.UpdateProductById(id, request);
        return Ok(order);
    }
}