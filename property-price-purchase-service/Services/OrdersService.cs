using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IOrdersService
{
    IEnumerable<Order> GetOrders();
}

public class OrdersService : IOrdersService
{
    private readonly PostgreSQLDbContext _dbContext;
    
    public OrdersService(PostgreSQLDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IEnumerable<Order> GetOrders()
    {
        return _dbContext.Orders;
    }
} 