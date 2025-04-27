namespace property_price_cosmos_db.Models;

public class TransactionsMetadata
{
    public required int TotalCount { get; set; }
    public required decimal AmountMean { get; set; }
    public required decimal AmountSum { get; set; }
}
