using Microsoft.AspNetCore.Mvc;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace property_price_cosmos_db.Controllers;

[ApiController]
[Route("api")]
public class TransactionsController: ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpGet("transactions")]
    public async Task<IActionResult> List()
    {
        return Ok(await _transactionService.GetMultipleAsync("SELECT * FROM c"));
    }
    [HttpGet("transactions/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        return Ok(await _transactionService.GetAsync(id));
    }
    
    [HttpPost("transactions")]
    public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
    {

        transaction.Id = Guid.NewGuid().ToString();
        await _transactionService.AddAsync(transaction);
        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);

    }
}