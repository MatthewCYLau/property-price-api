using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class OrderRequest
{
    public OrderRequest(string reference)
    {
        Reference = reference;
    }

    [Required]
    public string Reference { get; set; }
}