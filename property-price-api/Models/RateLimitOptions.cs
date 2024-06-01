namespace property_price_api.Models;

public class RateLimitOptions
{
    public const string CustomRateLimit = "CustomRateLimit";
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int ReplenishmentPeriod { get; set; }
    public int QueueLimit { get; set; }
    public int SegmentsPerWindow { get; set; }
    public int TokensPerPeriod { get; set; }
    public bool AutoReplenishment { get; set; }
}