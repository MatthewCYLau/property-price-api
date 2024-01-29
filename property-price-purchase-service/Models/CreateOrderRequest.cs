using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class CreateOrderRequest
{
    [Required]
    public string Reference { get; set; }
}