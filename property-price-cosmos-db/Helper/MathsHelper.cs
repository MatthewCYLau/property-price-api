namespace property_price_cosmos_db.Helper;

public static class MathsHelper
{
    public static bool IsEven(int input)
    {
        return input % 2 == 0;
    }

    public static async Task<int> GetRandomNumberInclusiveAsync(int min, int max)
    {
        return await Task.Run(() =>
        {
            Random random = new();
            int randomNumber = random.Next(min, max + 1);
            int randomWaitSeconds = random.Next(1, 5);
            Task.Delay(randomWaitSeconds).Wait();
            return randomNumber;
        });
    }
}
