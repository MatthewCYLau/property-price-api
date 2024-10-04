namespace property_price_cosmos_db.Models;

public class CreateCosmosUserRequest
{
    public required string Name { get; set; }
    public required DateTime DateOfBirth { get; set; }
}
