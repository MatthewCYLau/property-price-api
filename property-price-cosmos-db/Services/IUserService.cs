using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface IUserService
{
    // Task<User?> GetUserByIdAsync(string id);
    Task AddUserAsync(CosmosUser item);
}
