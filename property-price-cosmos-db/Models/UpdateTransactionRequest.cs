namespace property_price_cosmos_db.Models;

public class UpdateTransactionRequest
{
    public decimal Amount { get; set; }

    public string Description { get; set; }

    public bool Completed { get; set; }

}
