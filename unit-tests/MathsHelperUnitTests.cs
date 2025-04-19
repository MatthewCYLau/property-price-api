using property_price_cosmos_db.Helper;

namespace unit_tests;

public class MathsHelperUnitTests
{
    [Test]
    public async Task GetRandomNumberInclusiveAsync_Should()
    {

        var res = await MathsHelper.GetRandomNumberInclusiveAsync(1, 100);
        Assert.That(res, Is.TypeOf<int>());
    }

    [Test]
    public async Task GetSumOfRandomNumbersAsync_Should()
    {

        var res = await MathsHelper.GetSumOfRandomNumbersAsync(3);
        Assert.That(res, Is.TypeOf<int>());
    }
}
