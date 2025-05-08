namespace property_price_cosmos_db.Helper;

public static class TimeHelper
{
    public static DateTime GetCurrentTimeByTimeZoneId(string timeZoneId)
    {
        if (IsValidTimeZoneId(timeZoneId))
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime targetLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, targetTimeZone);
            return targetLocalTime;
        }
        else
        {
            return DateTime.UtcNow;
        }
    }

    public static bool IsValidTimeZoneId(string timeZoneId)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger(string.Empty);
        foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
        {
            if (z.Id == timeZoneId)
            {

                logger.LogInformation("{0} is a valid time zone ID", timeZoneId);
                return true;
            }
        }
        logger.LogWarning("{0} is an invalid time zone ID!", timeZoneId);
        return false;
    }
}
