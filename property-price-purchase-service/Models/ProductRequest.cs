using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class ProductRequest
{
    [Required]
    public string Name { get; set; }

    public ProductRequest(string name)
    {
        Name = name;
    }
}