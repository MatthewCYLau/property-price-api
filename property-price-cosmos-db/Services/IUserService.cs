using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface IUserService
{
    Task<CosmosUser?> GetUserById(string id);
    Task AddUserAsync(CosmosUser item);
    Task<IEnumerable<CosmosUser>> GetUsers(DateTime? fromDate, DateTime? toDate);
    Task<bool> DeleteUserById(string id);
    Task<CosmosUser> UpdateUserById(string id, UpdateCosmosUserRequest request);
}
