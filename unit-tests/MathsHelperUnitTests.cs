using property_price_cosmos_db.Helper;

namespace unit_tests;

public class MathsHelperUnitTests
{
    [Test]
    public async Task GetRandomNumberInclusiveAsync_ShouldAsync()
    {

        var res = await MathsHelper.GetRandomNumberInclusiveAsync(1, 100);
        Assert.That(res, Is.TypeOf<int>());
    }
}
