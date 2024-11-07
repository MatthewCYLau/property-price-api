using Microsoft.AspNetCore.Mvc;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace property_price_cosmos_db.Controllers;

[ApiController]
[Route("api")]
public class PaymentRequestsController(
    IPaymentRequestService paymentRequestService
        ) : ControllerBase
{
    private readonly IPaymentRequestService _paymentRequestService = paymentRequestService;

    [HttpPost("payment-requests")]
    public async Task<IActionResult> CreatePaymentRequest([FromBody] PaymentRequest request)
    {
        request.Id = Guid.NewGuid();
        var result = await _paymentRequestService.CreatePaymentRequest(request);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok(request);
    }
}
