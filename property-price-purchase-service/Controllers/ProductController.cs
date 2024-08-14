using Microsoft.AspNetCore.Mvc;
using property_price_purchase_service.Models;
using property_price_purchase_service.Services;

namespace property_price_purchase_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductsService _productsService;
    private readonly ILogger _log;


    public ProductController(IProductsService productsService, ILogger<ProductController> log)
    {
        _productsService = productsService;
        _log = log;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _productsService.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<Order> GetProductById(int id)
    {
        _log.LogInformation("Get product by ID {id}", id);
        var product = _productsService.GetProductById(id);
        if (product != null)
        {
            return Ok(product);
        }
        else
        {
            return NotFound();
        }

    }

    [HttpPost]
    public IActionResult CreateProduct(ProductRequest request)
    {
        if (!ProductsConstant.ProductsCatalog.ContainsKey(request.Name))
        {
            return BadRequest($"Product name {request.Name} not valid!");
        }

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
        var order = _productsService.UpdateProductById(id, request);
        return Ok(order);
    }
}