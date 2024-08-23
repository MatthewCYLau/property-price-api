namespace property_price_api.Models;

public class CreatePropertyErrors
{
    public static Error DailyLimitReached(int limit) => new(
       "CreateProperty.DailyLimitReached", $"Daily limit of {limit} reached");
}
