namespace property_price_cosmos_db.Models;

public class TransactionErrors
{
    public static Error InvalidUserId(string id) => new(
    "TransactionErrors.InvalidUserId", $"Invalid user ID {id}");
    public static Error InvalidTransactionId(string id) => new(
"TransactionErrors.InvalidTransactionId", $"Invalid transaction ID {id}");
}
