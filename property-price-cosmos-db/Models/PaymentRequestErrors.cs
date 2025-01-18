namespace property_price_cosmos_db.Models;

public class PaymentRequestErrors
{

    public static Error InvalidUserId(string userId) => new(
"PaymentRequestErrors.InvalidUserId", $"Invalid user Id {userId}.");
    public static Error DebtorInsufficientFund(string debtorUserId) => new(
"PaymentRequestErrors.DebtorInsufficientFund", $"Debtor with user Id {debtorUserId} has insufficient fund.");

    public static Error CreditorAndDebtorIdentical(string creditorUserId, string debtorUserId) => new(
"PaymentRequestErrors.CreditorAndDebtorIdentical", $"Creditor user Id {creditorUserId} cannot be same as debtor user ID {debtorUserId}");
}
