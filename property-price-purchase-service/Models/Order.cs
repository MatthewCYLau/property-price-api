using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace property_price_purchase_service.Models;

public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }
    [Column(TypeName = "varchar(100)")]
    public string Reference { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}