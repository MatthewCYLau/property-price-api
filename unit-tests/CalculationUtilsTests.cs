using property_price_common;

namespace unit_tests;

public class CalculationUtilsTests
{
    [Test]
    public void ShouldReturnRandomInt()
    {
        Assert.That(typeof(int), Is.EqualTo(CalculationUtils.GenerateRandomInteger(0, 10).GetType()));
    }
}
