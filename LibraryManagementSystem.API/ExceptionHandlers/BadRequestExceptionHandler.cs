using LibraryManagementSystem.Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace LibraryManagementSystem.API.ExceptionHandlers
{
    public class BadRequestExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<BadRequestExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger
            ,IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext
            , Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not BadRequestException badRequestException) return false;

            _logger.LogWarning(badRequestException, "An Bad Request exception has occured: {Message}"
                , badRequestException.Message);


            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Detail = badRequestException.Message,
                Status = (int)HttpStatusCode.BadRequest,
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
