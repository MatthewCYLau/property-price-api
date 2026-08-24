namespace property_price_api.Models;

public class BaseOopPerson
{
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (!string.IsNullOrEmpty(value) && !char.IsUpper(value[0]))
            {
                throw new ArgumentException("Name cannot be empty or whitespace. and must start with uppercase", nameof(value));
            }
            _name = value;
        }
    }
    
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