using property_price_api.Helpers;

namespace unit_tests;

public class CalculationsUnitTests
{

    [Test]
    public void ShouldReturnCorrectMeanSuggestedPrice()
    {
        
        var percentages = new List<int> { 10, 15, -10};
        const int askingPrice = 200000;
        Assert.That( 210000, Is.EqualTo(Calculations.MeanSuggestedPrice(percentages, askingPrice)));
    }
}