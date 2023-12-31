﻿using AutoMapper;
using Microsoft.Extensions.Options;
using property_price_api.Data;
using property_price_api.Helpers;
using property_price_api.Models;
using property_price_api.Profiles;
using property_price_api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PropertyPriceApiDatabaseSettings>(
    builder.Configuration.GetSection("PropertyPriceApiDatabase"));
builder.Services.AddSingleton(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<PropertyPriceApiDatabaseSettings>>().Value;
    return new MongoDbContext(
        Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING") ??
        settings.ConnectionString,
        settings.DatabaseName);
});

builder.Services.Configure<CloudPubSubOptions>(
    builder.Configuration.GetSection(CloudPubSubOptions.CloudPubSub));

builder.Services.AddSingleton<IPropertyService, PropertyService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPriceSuggestionService, PriceSuggestionService>();
builder.Services.AddSingleton<IIngestJobService, IngestJobService>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IGoogleCloudStorageService, GoogleCloudStorageService>();
builder.Services.AddSingleton<ICloudPubSubService, CloudPubSubService>();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Auto Mapper Configurations
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new UserProfile());
    mc.AddProfile(new PropertyProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddHttpContextAccessor();

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

app.UseMiddleware<JwtMiddleware>();

await app.Services.GetRequiredService<IPropertyService>().CreateSeedProperties();

app.MapGet("/ping", () => "pong!")
.WithDescription("Ping uptime check")
.WithOpenApi();

app.Run();

