namespace property_price_api.Helpers;

public static class Calculations
{
    public static int MeanSuggestedPrice(List<int> percentages, int askingPrice)
    {
        var meanSuggestedPrice = (int)percentages.Select(i => ((decimal)i / 100 + 1) * askingPrice).Sum() / percentages.Count;
        return meanSuggestedPrice;
    }
}