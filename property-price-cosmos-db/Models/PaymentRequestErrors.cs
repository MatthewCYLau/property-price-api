namespace property_price_cosmos_db.Models;

public class PaymentRequestErrors
{
    public static Error CreditorAndDebtorIdentical(string creditorUserId, string debtorUserId) => new(
"PaymentRequestErrors.CreditorAndDebtorIdentical", $"Creditor user Id {creditorUserId} cannot be same as debtor user ID {debtorUserId}");
}
