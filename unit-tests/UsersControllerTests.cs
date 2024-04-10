using Moq;
using property_price_api.Controllers;
using property_price_api.Models;
using property_price_api.Services;

namespace unit_tests;

public class UsersControllerTests
{
    [Test]
    public async Task GetUsersShould()
    {
        var newUser = new UserDto("1", "hello@example.com", "Renter");
        var mockUsersService = new Mock<IUserService>();
        mockUsersService.Setup(x => x.GetUsers()).Returns(Task.FromResult(new List<UserDto> {newUser}));
        var usersController = new UsersController(mockUsersService.Object);
        var users = await usersController.Get();
        Assert.That(users.Count == 1);
    }
}