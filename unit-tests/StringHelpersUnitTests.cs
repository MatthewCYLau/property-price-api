using property_price_api.Helpers;

namespace unit_tests
{
	public class StringHelpersUnitTests
	{
        [Test]
        public void ShouldReturnTrueValidUkPostcode()
        {
            Assert.IsTrue(StringHelpers.IsValidateUkPostcode("SE21 7LG"));
        }

        [Test]
        public void ShouldReturnFalseInvalidUkPostcode()
        {
            Assert.IsFalse(StringHelpers.IsValidateUkPostcode("foo bar"));
        }
    }
}

