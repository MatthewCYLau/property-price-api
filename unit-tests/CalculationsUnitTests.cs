using property_price_api.Helpers;

namespace unit_tests;

public class CalculationsUnitTests
{

    [Test]
    public void ShouldReturnCorrectMeanSuggestedPrice()
    {
        
        var percentages = new List<int> { 80, 90, 100, 110 };
        const int askingPrice = 200000;
        Assert.That( 190000, Is.EqualTo(Calculations.MeanSuggestedPrice(percentages, askingPrice)));
    }
}