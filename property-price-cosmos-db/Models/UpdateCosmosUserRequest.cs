namespace property_price_cosmos_db.Models;

public class UpdateCosmosUserRequest
{
    public required string Name { get; set; }
    public required DateTime DateOfBirth { get; set; }
}
