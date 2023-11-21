using property_price_ingest;
using property_price_ingest.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ScopedBackgroundService>();
builder.Services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

IHost host = builder.Build();
host.Run();
