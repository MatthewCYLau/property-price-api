using property_price_api.Helpers;

namespace unit_tests;

public class CalculationsUnitTests
{

    [Test]
    public void ShouldReturnMeanSuggestedPriceAboveAskingPrice()
    {
        
        var percentages = new List<int> { 10, 15, -10, 20};
        const int askingPrice = 200000;
        Assert.That( Calculations.MeanSuggestedPrice(percentages, askingPrice), Is.EqualTo(217500));
    }
    
    [Test]
    public void ShouldReturnMeanSuggestedPriceBelowAskingPrice()
    {
        
        var percentages = new List<int> { -10, 15, -10, -20};
        const int askingPrice = 200000;
        Assert.That( Calculations.MeanSuggestedPrice(percentages, askingPrice), Is.EqualTo(187500));
    }
}