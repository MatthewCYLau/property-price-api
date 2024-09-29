namespace property_price_cosmos_db.Models;

public class TransactionErrors
{
    public static Error InvalidUserId(string id) => new(
    "TransactionErrors.InvalidUserId", $"Invalid user ID {id}");
}
