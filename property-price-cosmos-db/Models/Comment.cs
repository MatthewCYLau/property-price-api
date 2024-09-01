namespace property_price_cosmos_db.Models;

public class Comment(Guid id, string description)
{
    public Guid Id { get; set; } = id;
    public string Description { get; set; } = description;
}