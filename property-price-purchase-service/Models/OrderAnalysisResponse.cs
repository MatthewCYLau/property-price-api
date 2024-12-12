namespace property_price_purchase_service.Models;

public class OrderAnalysisResponse
{
    public required int ProductId { get; set; }
    public int TotalQuantity { get; set; }
    public double AverageQuantity { get; set; }
}
