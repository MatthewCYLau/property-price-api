using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class OrderRequest
{
    public OrderRequest(string reference, int productId)
    {
        Reference = reference;
        ProductId = productId;
    }

    [Required]
    public string Reference { get; set; }
    public int ProductId { get; set; }
}