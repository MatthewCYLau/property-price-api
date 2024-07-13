using System.ComponentModel.DataAnnotations;

namespace property_price_purchase_service.Models;

public class OrderRequest
{
    public OrderRequest(string reference, int productId, int quantity)
    {
        Reference = reference;
        ProductId = productId;
        Quantity = quantity;
    }

    [Required]
    public string Reference { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

}