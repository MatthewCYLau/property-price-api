using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class UpdateProductPriceRequest
{
    [Required]
    public int Magnitude { get; set; }
}
