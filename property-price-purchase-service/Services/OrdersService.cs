using AutoMapper;
using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IOrdersService
{
    IEnumerable<Order> GetOrders();
    void CreateOrder(CreateOrderRequest request);
    void DeleteOrderById(int id);
    Order GetOrderById(int id);
}

public class OrdersService : IOrdersService
{
    private readonly PostgreSQLDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public OrdersService(PostgreSQLDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public IEnumerable<Order> GetOrders()
    {
        return _dbContext.Orders;
    }

    public void CreateOrder(CreateOrderRequest request)
    {
        var order = _mapper.Map<Order>(request);
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();
    }
    
    public void DeleteOrderById(int id)
    {
        var order = GetOrderById(id);
        _dbContext.Orders.Remove(order);
        _dbContext.SaveChanges();
    }
    
    public Order GetOrderById(int id)
    {
        var user = _dbContext.Orders.Find(id);
        if (user == null) throw new KeyNotFoundException("Order not found");
        return user;
    }
} 