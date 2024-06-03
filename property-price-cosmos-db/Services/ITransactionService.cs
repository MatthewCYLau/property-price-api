using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMultipleAsync(bool isComplete);
    Task<Transaction?> GetAsync(string id);
    Task AddAsync(Transaction item);
    Task<Transaction> UpdateAsync(string id, Transaction item);
    Task DeleteAsync(string id);
}