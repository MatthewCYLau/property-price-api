namespace property_price_cosmos_db.Services;

public class FactoryMiddleware(ILogger<FactoryMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);
        logger.LogInformation("Request agent {0}", context.Request.Headers.UserAgent);
    }
}
