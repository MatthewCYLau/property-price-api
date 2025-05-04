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
            Task.Delay(random.Next(1, 5) * 1_000).Wait();
            return randomNumber;
        });
    }

    public static async Task<int> GetSumOfRandomNumbersAsync(int count)
    {

        List<Task<int>> tasks = [];

        for (int i = 0; i < count; i++)
        {
            tasks.Add(GetRandomNumberInclusiveAsync(1, 100));
        }

        List<int> result = [.. await Task.WhenAll(tasks)];
        return result.Sum();
    }


    public static List<int> CreateListIntSwapFirstLast(int listSize)
    {
        var list = Enumerable.Range(1, listSize).ToList();
        (list[^1], list[0]) = (list[0], list[^1]);
        return list;
    }
}
