using AutoMapper;
using property_price_purchase_service.Data;
using property_price_purchase_service.Profiles;
using property_price_purchase_service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<PostgreSQLDbContext>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IProductsService, ProductsService>();


var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new OrderProfile());
    mc.AddProfile(new ProductProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Configure CORS
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").WithMethods("GET", "POST", "DELETE", "PUT", "PATCH")
            .AllowAnyHeader())
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/ping", () => "pong!")
.WithDescription("Ping uptime check")
.WithOpenApi();

app.Run();