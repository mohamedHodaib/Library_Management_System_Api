using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace LibraryManagementSystem.API.Filters.ActionFilters
{
    public class LogPerformanceFilterAttribute : IAsyncActionFilter
    {
        private readonly ILogger<LogPerformanceFilterAttribute> _logger;
        private const string StopwatchKey = "Stopwatch";

        public LogPerformanceFilterAttribute(ILogger<LogPerformanceFilterAttribute> logger)
        {
            _logger = logger;
        }



        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //run before action Execution Logic
            //Start stop watch and store it in the context to use it later at OnActionExecuted Method
            Stopwatch stopWatch = Stopwatch.StartNew();
            context.HttpContext.Items[StopwatchKey] = StopwatchKey;

            var action = context.RouteData.Values["Action"];
            var controller = context.RouteData.Values["Controller"];

            _logger.LogInformation("Executing an action {Action} on controller {Controller}.", action, controller);

            await next();

            //runs After Action Execution Logic
            if (context.HttpContext.Items[StopwatchKey] is Stopwatch stopwatch)
            {
                stopwatch.Stop();

                var timeElapsed = stopwatch.ElapsedMilliseconds;

                _logger.LogInformation("Finished action {Action} on controller {Controller} in {TimeElapsed}ms."
                    , action
                    , controller
                    , timeElapsed
                );
            }
        }
    }
}
