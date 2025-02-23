using Microsoft.EntityFrameworkCore;

namespace property_price_kafka_service.Models;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<InventoryUpdateRequest> InventoryUpdates { get; set; }
}
