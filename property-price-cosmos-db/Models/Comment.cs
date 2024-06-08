namespace property_price_cosmos_db.Models;

public class Comment(string id, string description)
{
    public string? Id { get; set; } = id;
    public string Description { get; set; } = description;
}