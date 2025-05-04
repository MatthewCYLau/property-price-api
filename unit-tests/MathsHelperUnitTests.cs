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

    [Test]
    public void TestCreateListIntSwapFirstLast()
    {
        Assert.Multiple(() =>
        {
            Assert.That(MathsHelper.CreateListIntSwapFirstLast(4), Is.EqualTo(new List<int> { 4, 2, 3, 1 }));
            Assert.That(MathsHelper.CreateListIntSwapFirstLast(5), Is.EqualTo(new List<int> { 5, 2, 3, 4, 1 }));
        });
    }
}
