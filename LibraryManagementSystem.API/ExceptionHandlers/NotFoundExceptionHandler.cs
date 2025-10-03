using LibraryManagementSystem.Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagementSystem.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<NotFoundExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger
            , IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException notFoundException)
            {
                return false; // This handler is not responsible
            }

            _logger.LogError(notFoundException, "A Not Found exception has occured: {Message}"
                , notFoundException.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var problemDetails = new ProblemDetails
            {
                Title = "Not Found",
                Detail = notFoundException.Message,
                Status = (int)HttpStatusCode.NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            };

            await _problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });

            return true; 
        }
    }
}