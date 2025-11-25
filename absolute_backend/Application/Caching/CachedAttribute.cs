using Application.Abstractions.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Application.Caching.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _durationSeconds;

    public CachedAttribute(int durationSeconds = 60)
    {
        _durationSeconds = durationSeconds;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

        var key = BuildCacheKey(context);

        var cached = await cache.GetAsync<string>(key);
        if (cached != null)
        {
            context.Result = new ContentResult
            {
                Content = cached,
                ContentType = "application/json",
                StatusCode = 200
            };
            return;
        }

        var executed = await next();

        if (executed.Result is ObjectResult ok && ok.Value != null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
            await cache.SetAsync(key, json, TimeSpan.FromSeconds(_durationSeconds));
        }
    }

    private string BuildCacheKey(ActionExecutingContext ctx)
    {
        var sb = new StringBuilder();

        sb.Append($"{ctx.HttpContext.Request.Path}");

        foreach (var (key, value) in ctx.ActionArguments)
            sb.Append($"|{key}-{value}");

        return sb.ToString().ToLower();
    }
}