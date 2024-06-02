using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMultipleAsync(string query);
    Task<Transaction> GetAsync(string id);
    Task AddAsync(Transaction item);
    Task UpdateAsync(string id, Transaction item);
    Task DeleteAsync(string id);
}