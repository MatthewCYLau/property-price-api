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

        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<List<UserDto>> Get() =>
            await _userService.GetUsers();

        //[HttpGet("{id:length(24)}")]
        //public async Task<ActionResult<Property>> Get(string id)
        //{
        //    var property = await _propertyService.GetAsync(id);

        //    if (property is null)
        //    {
        //        return NotFound();
        //    }

        //    return property;
        //}


        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {

            var existingUser = await _userService.GetUserByEmail(createUserDto.Email);
            if (existingUser != null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "User already exists!" });
            UserDto userDto = await _userService.CreateUser(createUserDto);

            return CreatedAtAction(nameof(Get), new { id = userDto.Id }, userDto);
        }

        //[HttpPut("{id:length(24)}")]
        //public async Task<IActionResult> Update(string id, Property updatedProperty)
        //{
        //    var property = await _propertyService.GetAsync(id);

        //    if (property is null)
        //    {
        //        return NotFound();
        //    }

        //    updatedProperty.Id = property.Id;

        //    await _propertyService.UpdateAsync(id, updatedProperty);

        //    return NoContent();
        //}

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.GetUserById(id);

            if (user is null)
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

