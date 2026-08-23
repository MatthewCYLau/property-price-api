namespace property_price_api.Models;

public class OopPerson: BaseOopPerson
{
 
    public int Age { get; set; }

    public OopPerson(string name, int age, ILogger<OopPerson> logger) 
        : base(name, logger) // Pass logger up to Person
    {
        Age = age;
    }

    public override void LogName()
    {
        base.LogName();
        Logger.LogInformation("Oop person age: {age}", Age);
    }
    
}