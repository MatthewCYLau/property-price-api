using Microsoft.AspNetCore.Mvc;
using property_price_cosmos_db.Models;
using property_price_cosmos_db.Services;

namespace property_price_cosmos_db.Controllers;

[ApiController]
[Route("api")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateCosmosUserRequest request)
    {
        CosmosUser user = new() { Id = Guid.NewGuid(), Name = request.Name, DateOfBirth = request.DateOfBirth };
        await _userService.AddUserAsync(user);
        return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(DateTime? fromDate, DateTime? toDate)
    {

        var users = await _userService.GetUsers(fromDate, toDate);
        return Ok(users);
    }


    [HttpGet("users/{id}")]
    public async Task<ActionResult<CosmosUser>> GetUserById(string id)
    {
        var user = await _userService.GetUserById(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUserById(string id)
    {
        var res = await _userService.DeleteUserById(id);

        if (!res)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPut("users/{id}")]
    public async Task<ActionResult<CosmosUser>> UpdateUser(string id, [FromBody] UpdateCosmosUserRequest request)
    {
        var user = await _userService.UpdateUserById(id, request);
        return Ok(user);
    }
}
