using Application.Abstractions.Caching;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Caching.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    public string Key { get; }
    public int DurationSeconds { get; }

    public CachedAttribute(string key, int durationSeconds = 60)
    {
        Key = key;
        DurationSeconds = durationSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        var cached = await cache.GetAsync<object>(Key);
        if (cached != null)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.JsonResult(cached);
            return;
        }

        var executedContext = await next();

        if (executedContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult ok &&
            ok.Value is not null)
        {
            await cache.SetAsync(Key, ok.Value, TimeSpan.FromSeconds(DurationSeconds));
        }
    }
}