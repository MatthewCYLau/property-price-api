namespace property_price_common;

public static class CalculationUtils
{
    private static readonly Random random = new();

    public static int GenerateRandomInteger(int
 minValue, int maxValue)
    {
        return random.Next(minValue, maxValue + 1);
    }
}
