using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount);
    Task<Transaction?> GetAsync(string id);
    Task AddAsync(Transaction item);
    Task<Transaction> UpdateAsync(string id, Transaction item);
    Task<Transaction> UpdateTransactionCommentsAsync(string id, Comment comment);
    Task DeleteAsync(string id);
}