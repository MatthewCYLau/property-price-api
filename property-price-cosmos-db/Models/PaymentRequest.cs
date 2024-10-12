namespace property_price_cosmos_db.Models;

public class PaymentRequest
{
    public Guid Id { get; set; }
    public required Guid CreditorUserId { get; set; }
    public required Guid DebtorUserId { get; set; }
}

