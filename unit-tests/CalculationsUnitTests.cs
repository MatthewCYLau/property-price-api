using property_price_api.Helpers;

namespace unit_tests;

public class CalculationsUnitTests
{

    [Test]
    public void ShouldReturnMeanSuggestedPriceAboveAskingPrice()
    {
        
        var percentages = new List<int> { 10, 15, -10, 20};
        const int askingPrice = 200000;
        Assert.That( 217500, Is.EqualTo(Calculations.MeanSuggestedPrice(percentages, askingPrice)));
    }
    
    [Test]
    public void ShouldReturnMeanSuggestedPriceBelowAskingPrice()
    {
        
        var percentages = new List<int> { -10, 15, -10, -20};
        const int askingPrice = 200000;
        Assert.That( 187500, Is.EqualTo(Calculations.MeanSuggestedPrice(percentages, askingPrice)));
    }
}