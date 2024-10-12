using property_price_cosmos_db.Models;

namespace property_price_cosmos_db.Services;

public interface IPaymentRequestService
{
    Task<Result> CreatePaymentRequest(PaymentRequest request);
}
