using LibraryManagementSystem.Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryBookManagementSystem.API.Middlewares.ExceptionHandlers
{
    public class ConflictExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ConflictExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public ConflictExceptionHandler(ILogger<ConflictExceptionHandler> logger
            ,IProblemDetailsService problemDetailsService)
        { 
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ConflictException conflictException)
            {
                return false;
            }

            _logger.LogWarning(conflictException, "An Conflict exception has occured: {Message}", conflictException.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

            var problemDetails = new ProblemDetails
            {
                Title = "Conflict",
                Detail = conflictException.Message,
                Status = (int)HttpStatusCode.Conflict,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
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