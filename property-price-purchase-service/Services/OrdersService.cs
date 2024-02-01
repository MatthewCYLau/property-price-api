using AutoMapper;
using Microsoft.EntityFrameworkCore;
using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IOrdersService
{
    IEnumerable<Order> GetOrders();
    void CreateOrder(OrderRequest request);
    void DeleteOrderById(int id);
    Order UpdateOrderById(int id, OrderRequest request);
    Order GetOrderById(int id);
}

public class OrdersService : IOrdersService
{
    private readonly PostgreSQLDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IProductsService _productsService;
    
    public OrdersService(PostgreSQLDbContext dbContext, IMapper mapper, IProductsService productsService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _productsService = productsService;
    }
    
    public IEnumerable<Order> GetOrders()
    {
        return _dbContext.Orders.Include(x => x.Product);
    }

    public void CreateOrder(OrderRequest request)
    {
        var order = _mapper.Map<Order>(request);
        order.Product = _productsService.GetProductById(1);
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
    
    public Order UpdateOrderById(int id, OrderRequest request)
    {
        var order = _dbContext.Orders.Find(id);
        _mapper.Map(request, order);
        _dbContext.Orders.Update(order);
        _dbContext.SaveChanges();
        return order;
    }
} 