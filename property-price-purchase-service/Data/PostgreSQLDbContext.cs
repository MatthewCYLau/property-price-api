using Microsoft.EntityFrameworkCore;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Data;

public class PostgreSQLDbContext: DbContext
{
    private readonly IConfiguration Configuration;

    public PostgreSQLDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        const string sectionName = "PostgreSQLDatabase";
        options.UseNpgsql(Configuration.GetConnectionString(sectionName));
    }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    
    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: Base, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            ((Base)entityEntry.Entity).UpdatedDate = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((Base)entityEntry.Entity).CreatedDate = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}