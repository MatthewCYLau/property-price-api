using AutoMapper;
using Microsoft.EntityFrameworkCore;
using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IProductsService
{
    IEnumerable<Product> GetProducts();

    void CreateProduct(ProductRequest request);
    Product GetProductById(int id);
    void DeleteProductById(int id);
    Product UpdateProductById(int id, ProductRequest request);
}

public class ProductsService: IProductsService
{
    private readonly PostgreSQLDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public ProductsService(PostgreSQLDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    public IEnumerable<Product> GetProducts()
    {
        return _dbContext.Products.Include(x => x.Orders);
    }
    
    public void CreateProduct(ProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        _dbContext.Products.Add(product);
        _dbContext.SaveChanges();
    }
    
    public Product GetProductById(int id)
    {
        var product = _dbContext.Products.Find(id);
        if (product == null) throw new KeyNotFoundException("Product not found");
        return product;
    }
    
    public void DeleteProductById(int id)
    {
        var product = GetProductById(id);
        _dbContext.Products.Remove(product);
        _dbContext.SaveChanges();
    }
    
    public Product UpdateProductById(int id, ProductRequest request)
    {
        var product = _dbContext.Products.Find(id);
        _mapper.Map(request, product);
        _dbContext.Products.Update(product);
        _dbContext.SaveChanges();
        return product;
    }
}