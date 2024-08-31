using Microsoft.AspNetCore.Mvc;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace property_price_cosmos_db.Controllers;

[ApiController]
[Route("api")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> List([FromQuery] bool? isComplete, int? maxAmount)
    {
        return Ok(await _transactionService.GetMultipleAsync(isComplete, maxAmount));
    }
    [HttpGet("transactions/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var transaction = await _transactionService.GetAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        return Ok(transaction);
    }

    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
    {

        transaction.Id = Guid.NewGuid();
        await _transactionService.AddAsync(transaction);
        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPost("transactions/{id}/comments")]
    public async Task<IActionResult> UpdateTransactionComment(string id, [FromBody] Comment comment)
    {

        var transaction = await _transactionService.UpdateTransactionCommentsAsync(id, comment);
        return Ok(transaction);
    }

    [HttpDelete("transactions/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("transactions/{id}")]
    public async Task<IActionResult> Edit([FromBody] Transaction item)
    {
        var transaction = await _transactionService.UpdateAsync(item.Id.ToString(), item);
        return Ok(transaction);
    }
}