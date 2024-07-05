using System.Globalization;
using CsvHelper;
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

        private readonly IUserService _userService;

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

            if (UserTypes.UserTypesList.All(n => n != createUserRequest.UserType))
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                   new Response { Status = "Error", Message = "Invalid user type" });
            }

            var existingUser = await _userService.GetUserByEmail(createUserRequest.Email);
            if (existingUser != null)
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = "Error", Message = "User already exists!" });
            AuthenticateResponse res = await _userService.CreateUser(createUserRequest);

            return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
        }

        [Authorize]
        [HttpPatch("{id:length(24)}")]
        public async Task<IActionResult> UpdateUserById(string id, UpdateUserRequest updateUserRequest)
        {
            var _userDto = await _userService.GetUserById(id);

            if (_userDto is null)
            {
                return NotFound();
            }

            if (!await _userService.UpdateUserById(id, updateUserRequest))
            {
                return BadRequest(new { message = "Update user went wrong!" });
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteUserById(string id)
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

        [Authorize]
        [Produces("text/csv")]
        [HttpPost("export-csv")]
        public async Task<FileResult> ExportCsv()
        {
            using var memoryStream = new MemoryStream();
            var data = await _userService.GetUsers();

            await using (var streamWriter = new StreamWriter(memoryStream))
            await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                await csvWriter.WriteRecordsAsync(data);
            }

            return File(memoryStream.ToArray(), "text/csv", $"users-export-{DateTime.Now:yyyy-MM-dd}.csv");
        }
    }
}

