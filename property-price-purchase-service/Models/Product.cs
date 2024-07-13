using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace property_price_purchase_service.Models;

public class Product : Base
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }
    [Column(TypeName = "varchar(100)")]

    public string Name { get; set; }
    public double Price { get; set; }

    public ICollection<Order> Orders { get; set; }
}