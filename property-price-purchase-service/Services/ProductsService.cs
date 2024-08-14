using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using property_price_purchase_service.Data;
using property_price_purchase_service.Models;

namespace property_price_purchase_service.Services;

public interface IProductsService
{
    IEnumerable<Product> GetProducts();

    void CreateProduct(ProductRequest request);
    Product? GetProductById(int id);
    void DeleteProductById(int id);
    Product UpdateProductById(int id, ProductRequest request);
}

public class ProductsService : IProductsService
{
    private readonly IConfiguration _configuration;
    private readonly PostgreSQLDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly PostgreSqlDbOptions _options;


    public ProductsService(IConfiguration configuration, PostgreSQLDbContext dbContext, IMapper mapper, IOptions<PostgreSqlDbOptions> options)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _configuration = configuration;
        _options = options.Value;
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

    public Product? GetProductById(int id)
    {
        const string queryString = "SELECT * FROM \"Products\" WHERE \"ProductId\" = @productId;";
        // const string sectionName = "PostgreSQLDatabase";
        // using var connection = new NpgsqlConnection(_configuration.GetConnectionString(sectionName));
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        var command = new NpgsqlCommand(queryString, connection);
        command.Parameters.AddWithValue("@productId", id);

        {
            connection.Open();
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                var product = new Product();
                while (reader.Read())
                {
                    product.ProductId = (int)reader["ProductId"];
                    product.Name = reader["Name"].ToString();
                    product.Price = double.Parse(reader["Price"].ToString());
                    product.CreatedDate = (DateTime)reader["CreatedDate"];
                    product.UpdatedDate = (DateTime)reader["UpdatedDate"];

                }
                reader.Close();
                return product;
            }
            else
            {
                reader.Close();
                return null;
            }
        }
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