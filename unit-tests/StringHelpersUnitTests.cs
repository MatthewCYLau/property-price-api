using property_price_api.Helpers;

namespace unit_tests
{
    public class StringHelpersUnitTests
    {
        [Test]
        public void ShouldReturnTrueValidUkPostcode()
        {
            Assert.That(StringHelpers.IsValidateUkPostcode("SE21 7LG"), Is.True);
        }

        [Test]
        public void ShouldReturnFalseInvalidUkPostcode()
        {
            Assert.That(StringHelpers.IsValidateUkPostcode("foo bar"), Is.False);
        }
    }
}

