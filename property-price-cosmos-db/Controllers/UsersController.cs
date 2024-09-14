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
    public async Task<IActionResult> CreateUser([FromBody] CosmosUser user)
    {

        user.Id = Guid.NewGuid();
        await _userService.AddUserAsync(user);
        return Ok(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {

        var users = await _userService.GetUsers();
        return Ok(users);
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
}
