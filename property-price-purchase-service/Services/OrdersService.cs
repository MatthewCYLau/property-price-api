using AutoMapper;
using Microsoft.EntityFrameworkCore;
using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IOrdersService
{
    IEnumerable<Order> GetOrders();
    ProcessOrderResult CreateOrder(OrderRequest request);
    void DeleteOrderById(int id);
    Order UpdateOrderById(int id, OrderRequest request);
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
        var orders = _dbContext.Orders.Include(x => x.Product);
        var largeOrder = from i in orders.ToList()
                         where i.Quantity > 5
                         select i;
        foreach (Order i in largeOrder)
        {
            Console.Write($"Large order quantity {i.Reference} {i.Quantity}");
        }
        return orders;
    }

    public ProcessOrderResult CreateOrder(OrderRequest request)
    {

        if (request.Reference.Length > 10)
        {
            return ProcessOrderResult.NotProcessable();
        }

        var order = _mapper.Map<Order>(request);
        order.Product = _dbContext.Products.Find(request.ProductId);
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();
        return ProcessOrderResult.Success();
    }

    public void DeleteOrderById(int id)
    {
        var order = GetOrderById(id);

        if ((DateTime.Now - order.CreatedDate).TotalDays < 1)
        {
            throw new BadHttpRequestException("Cannot delete order created within one day");
        }

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