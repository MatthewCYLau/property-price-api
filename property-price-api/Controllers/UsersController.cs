using Microsoft.AspNetCore.Mvc;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Services;

namespace property_price_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        [Authorize]
        [HttpGet]
        public async Task<List<UserDto>> Get() =>
            await _userService.GetUsers();

        [Authorize]
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var userDto = await _userService.GetUserById(id);

            if (userDto is null)
            {
                return NotFound();
            }

            return userDto;
        }


        [HttpPost]
        public async Task<ActionResult<AuthenticateResponse>> CreateUser(CreateUserRequest createUserRequest)
        {

            var existingUser = await _userService.GetUserByEmail(createUserRequest.Email);
            if (existingUser != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });
            AuthenticateResponse res = await _userService.CreateUser(createUserRequest);

            return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
        }

        [Authorize]
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateUserById(string id, UpdateUserRequest updateUserRequest)
        {
            var _userDto = await _userService.GetUserById(id);

            if (_userDto is null)
            {
                return NotFound();
            }

            if (!await _userService.UpdateUserById(id, updateUserRequest))
            {
                return BadRequest(new { message = "Updatd user went wrong!" });
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userDto = await _userService.GetUserById(id);

            if (userDto is null)
            {
                return NotFound();
            }

            if (!await _userService.DeleteUser(id))
            {
                return BadRequest(new { message = "Delete user went wrong!" });
            }

            return NoContent();

        }
    }
}

