namespace property_price_api.Helpers;

public static class Calculations
{
    /// <summary>
    /// Returns mean suggested price
    /// </summary>
    /// <param name="percentages">Percentage</param>
    /// <param name="askingPrice">Asking price</param>
    /// <returns>Means suggested price</returns>
    public static int MeanSuggestedPrice(List<int> percentages, int askingPrice)
    {
        var meanSuggestedPrice = (int)percentages.Select(i => ((decimal)i / 100 + 1) * askingPrice).Sum() / percentages.Count;
        return meanSuggestedPrice;
    }
}