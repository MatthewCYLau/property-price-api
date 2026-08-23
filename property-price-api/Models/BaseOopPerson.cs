namespace property_price_api.Models;

public class BaseOopPerson
{
    public string Name { get; set; }
    protected readonly ILogger<BaseOopPerson> Logger;

    public BaseOopPerson(string name, ILogger<BaseOopPerson> logger)
    {
        Name = name;
        Logger = logger;
    }

    public virtual void LogName()
    {
        Logger.LogInformation("Base person name: {Name}", Name);
    }
}