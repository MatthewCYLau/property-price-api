namespace property_price_cosmos_db.Models;

public class GetTransactionsResponse
{
    public required IEnumerable<Transaction> Transactions { get; set; }
    public required List<CountByAmountResponse> CountByAmountResponse { get; set; } = [];
    public required TransactionsMetadata TransactionsMetadata { get; set; }
}
