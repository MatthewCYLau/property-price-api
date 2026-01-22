namespace property_price_cosmos_db.Models;

public class CosmosUserErrors
{
    public static Error UsernameAlreadyExists(string name) => new(
"CosmosUserErrors.UsernameAlreadyExists", $"Username already exists {name}.");
}
