using Newtonsoft.Json.Serialization;

namespace property_price_cosmos_db.Services;

public class CustomInitService(ILogger<CustomInitService> logger) : ICustomInitService
{
    private readonly ILogger _logger = logger;

    public void GetAssembly()
    {
        _logger.LogInformation(typeof(JsonProperty).Assembly.CodeBase);

    }
}
