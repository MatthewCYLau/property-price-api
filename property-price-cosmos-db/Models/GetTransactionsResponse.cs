namespace property_price_cosmos_db.Models;

public class GetTransactionsResponse
{
    public required IEnumerable<Transaction> Transactions { get; set; }
    public required int TransactionsCount { get; set; }
    public required decimal TransactionsAmountMean { get; set; }
}
