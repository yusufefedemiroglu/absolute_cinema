using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Diagnostics;

namespace Application.Filters
{
    public class LoggingActionFilter : IActionFilter
    {
        private readonly Serilog.ILogger _logger;
        private Stopwatch? _sw;

        public LoggingActionFilter()
        {
            _logger = Log.ForContext<LoggingActionFilter>();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _sw = Stopwatch.StartNew();

            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;

            var parameters = context.ActionArguments;

            _logger.Information("➡️ {Controller}/{Action} started. Params: {@Params}",
                controller, action, parameters);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _sw?.Stop();

            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;
            var statusCode = context.HttpContext.Response.StatusCode;

            _logger.Information("⬅️ {Controller}/{Action} finished in {Elapsed} ms. Status: {StatusCode}",
                controller,
                action,
                _sw?.ElapsedMilliseconds,
                statusCode);
        }
    }
}