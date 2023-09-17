namespace property_price_api.Helpers;

public static class Calculations
{
    public static decimal MeanSuggestedPrice(List<int> percentages, decimal askingPrice)
    {
        var meanSuggestedPrice = percentages.Select(i => (decimal)i / 100 * askingPrice).Sum() / percentages.Count;
        return meanSuggestedPrice;
    }
}