using Newtonsoft.Json;

namespace property_price_cosmos_db.Models;

public class PaymentRequest
{
    [JsonProperty(PropertyName = "id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "creditorUserId")]
    public required Guid CreditorUserId { get; set; }

    [JsonProperty(PropertyName = "debtorUserId")]
    public required Guid DebtorUserId { get; set; }

    [JsonProperty(PropertyName = "amount")]
    public required decimal Amount { get; set; }
}

