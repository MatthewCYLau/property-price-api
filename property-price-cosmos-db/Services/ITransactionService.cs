using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount, string? orderBy);
    Task<Transaction?> GetAsync(string id);
    Task AddAsync(Transaction item);
    Task<Transaction> UpdateAsync(string id, UpdateTransactionRequest request);
    Task<Transaction> UpdateTransactionAppendCommentsAsync(string id, Comment comment);
    Task<Transaction> UpdateCommentAsync(string transactionId, string commentId, UpdateCommentRequest request);
    Task DeleteAsync(string id);
    Task<Transaction> DeleteCommentAsync(string transactionId, string commentId);
    Task<AnalysisResponse> GetTransactionsAnalysisResponse();
}