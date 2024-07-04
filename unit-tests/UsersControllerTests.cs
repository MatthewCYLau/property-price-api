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
    
    [Test]
    public async Task GetUserByIdShould()
    {
        const string userId = "1";
        const string email = "hello@example.com";
        var newUser = new UserDto(userId, email, "Renter");
        var mockUsersService = new Mock<IUserService>();
        mockUsersService.Setup(x => x.GetUserById("1")).Returns(Task.FromResult(newUser));
        var usersController = new UsersController(mockUsersService.Object);
        var user = await usersController.GetUserById(userId);
        Assert.That(user.Value.Email == email);
    }
    
    [Test]
    public async Task ExportUsersCSvShould()
    {
        var newUser = new UserDto("1", "hello@example.com", "Renter");
        var mockUsersService = new Mock<IUserService>();
        mockUsersService.Setup(x => x.GetUsers()).Returns(Task.FromResult(new List<UserDto> {newUser}));
        var usersController = new UsersController(mockUsersService.Object);
        var fileResult = await usersController.ExportCsv();

        // Assert
        Assert.IsNotNull(fileResult);
        Assert.That(fileResult.ContentType, Is.EqualTo("text/csv"));
    }
}