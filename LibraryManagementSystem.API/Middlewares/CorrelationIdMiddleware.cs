using Serilog.Context;

namespace LibraryManagementSystem.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private const string CorrelationIdKey = "CorrelationId";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Try to get correlation Id if it exist in the request header
            if(!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();  
            }

            if(!context.Response.Headers.ContainsKey(CorrelationIdHeader))
            {
                //Add it to the response headers
                context.Response.Headers.Add(CorrelationIdHeader, correlationId);
            }
            
            // This makes 'CorrelationId' available to ALL subsequent logs in this request.
            using(LogContext.PushProperty(CorrelationIdKey, correlationId))
            {
                await _next(context);
            }

        }
    }
}
