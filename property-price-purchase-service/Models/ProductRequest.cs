using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class ProductRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public double Price { get; set; }

    public ProductRequest(string name, double price)
    {
        Name = name;
        Price = price;
    }
}