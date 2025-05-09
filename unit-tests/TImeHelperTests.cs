using property_price_cosmos_db.Helper;

namespace unit_tests;

public class TImeHelperTests
{
    [Test]
    public void TestIsValidTimeZoneId()
    {
        Assert.Multiple(() =>
        {
            Assert.That(TimeHelper.IsValidTimeZoneId("foo"), Is.False);
            Assert.That(TimeHelper.IsValidTimeZoneId("America/New_York"), Is.True);
        });
    }
}
