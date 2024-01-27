using Microsoft.EntityFrameworkCore;

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
}