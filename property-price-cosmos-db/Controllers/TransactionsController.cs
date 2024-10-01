using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace property_price_cosmos_db.Controllers;

[ApiController]
[Route("api")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IConfiguration _configuration;
    private readonly IAzureClientFactory<ServiceBusSender> _serviceBusSenderFactory;


    public TransactionsController(
        ITransactionService transactionService,
        IConfiguration configuration,
        IAzureClientFactory<ServiceBusSender> serviceBusSenderFactory)
    {
        _transactionService = transactionService;
        _configuration = configuration;
        _serviceBusSenderFactory = serviceBusSenderFactory;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> List([FromQuery] bool? isComplete, int? maxAmount, string? orderBy)
    {
        return Ok(await _transactionService.GetMultipleAsync(isComplete, maxAmount, orderBy));
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
        var result = await _transactionService.AddAsync(transaction);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        var _sender = _serviceBusSenderFactory.CreateClient("sbt-aks-storage-request-sender");
        var sql_value = "val1";
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(transaction)))
        {

            ApplicationProperties =
                {
                { "var1", sql_value }

            }
        };
        await _sender.SendMessageAsync(message);
        return CreatedAtAction(nameof(Get), new { id = transaction.Id }, transaction);
    }

    [HttpPost("transactions/{id}/comments")]
    public async Task<IActionResult> UpdateTransactionComment(string id, [FromBody] Comment comment)
    {

        var transaction = await _transactionService.UpdateTransactionAppendCommentsAsync(id, comment);
        return Ok(transaction);
    }

    [HttpDelete("transactions/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _transactionService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPut("transactions/{id}")]
    public async Task<IActionResult> Edit(string id, [FromBody] UpdateTransactionRequest request)
    {
        var transaction = await _transactionService.UpdateAsync(id, request);
        return Ok(transaction);
    }

    [HttpPut("transactions/{transactionId}/comments/{commentId}")]
    public async Task<IActionResult> EditComment(string transactionId, string commentId, [FromBody] UpdateCommentRequest request)
    {
        var transaction = await _transactionService.UpdateCommentAsync(transactionId, commentId, request);
        return Ok(transaction);
    }

    [HttpDelete("transactions/{transactionId}/comments/{commentId}")]
    public async Task<IActionResult> DeleteComment(string transactionId, string commentId)
    {
        var transaction = await _transactionService.DeleteCommentAsync(transactionId, commentId);
        return Ok(transaction);
    }

    [HttpGet("analysis/transactions")]
    public async Task<ActionResult<AnalysisResponse>> GetTransactionsAnalysis()
    {
        var analysis = await _transactionService.GetTransactionsAnalysisResponse();

        return Ok(analysis);
    }
    [HttpGet("secret")]
    public async Task<IActionResult> GetSecretFromAzureKeyVault()
    {
        return Ok(_configuration.GetConnectionString("MyDatabase"));
    }
}