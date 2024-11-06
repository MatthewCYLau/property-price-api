namespace property_price_cosmos_db.Models;

public class Comment(string description)
{
    public Guid Id { get; set; }
    public string Description { get; set; } = description;
}