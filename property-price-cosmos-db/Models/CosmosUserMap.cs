namespace property_price_cosmos_db.Models;

public static class CosmosUserMap
{
    public static readonly Dictionary<int, CosmosUser> CosmosUsers = new()
    {
        { 1, new CosmosUser() { Id = Guid.NewGuid(), Name = "Test user", DateOfBirth = DateTime.Parse("1990-01-01") } },
    };
}
