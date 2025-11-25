using Application.Abstractions.Caching;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Caching.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _pattern;

    public InvalidateCacheAttribute(string pattern)
    {
        _pattern = pattern;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();

        var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        await cache.RemoveAsync(_pattern);
    }
}