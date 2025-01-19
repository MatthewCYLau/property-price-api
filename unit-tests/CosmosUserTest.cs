using property_price_cosmos_db.Models;

namespace unit_tests;

public class CosmosUserTest
{

    [Test]
    public void Test_ValidCosmosUserDefaultBalance()
    {
        var person = new CosmosUser() { Id = Guid.NewGuid(), Name = "Test user", DateOfBirth = DateTime.Parse("1990-01-01") };
        Assert.That(person.Balance, Is.EqualTo(0));
    }
}
