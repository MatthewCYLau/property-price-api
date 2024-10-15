using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMultipleAsync(bool? isComplete, int? maxAmount, string? orderBy);
    Task<IEnumerable<Transaction>> GetTransactionsByUserId(string id);
    Task<Transaction?> GetAsync(string id);
    Task CreateSeedTransactions();
    Task<Result> AddAsync(Transaction item);
    Task<Transaction> UpdateAsync(string id, UpdateTransactionRequest request);
    Task<Transaction> UpdateTransactionAppendCommentsAsync(string id, Comment comment);
    Task<Transaction> UpdateCommentAsync(string transactionId, string commentId, UpdateCommentRequest request);
    Task DeleteAsync(string id);
    Task<IEnumerable<Transaction>> ReadTransactionBlobAsync(string id);
    Task<Transaction> DeleteCommentAsync(string transactionId, string commentId);
    Task<AnalysisResponse> GetTransactionsAnalysisResponse();
    Task<Transaction> UpdateTrasnscationCompleteState(string id, bool isComplete);
    Task<Uri> ExportTransactionsByUserId(string id);
}