namespace property_price_kafka_service.Models;

public class InventoryUpdateRequest
{
    public int Id { get; set; }

    public string ProductId { get; set; }

    public int Quantity { get; set; }
}
