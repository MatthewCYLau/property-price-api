namespace property_price_cosmos_db.Helper;

public class TimeHelper
{
    public static DateTime GetCurrentTimeByTimeZoneId(string timeZoneId)
    {
        DateTime utcNow = DateTime.UtcNow;
        TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime targetLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, targetTimeZone);
        return targetLocalTime;
    }
}
