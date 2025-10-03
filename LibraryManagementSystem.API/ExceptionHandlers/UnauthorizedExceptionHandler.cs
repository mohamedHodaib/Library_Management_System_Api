using LibraryBookManagementSystem.API.Middlewares.ExceptionHandlers;
using LibraryManagementSystem.Business.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LibraryManagementSystem.API.ExceptionHandlers
{
    public class UnauthorizedExceptionHandler : IExceptionHandler
    {

        private readonly ILogger<UnauthorizedExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;

        public UnauthorizedExceptionHandler(ILogger<UnauthorizedExceptionHandler> logger
            , IProblemDetailsService problemDetailsService)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
        }


        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext
            , Exception exception
            , CancellationToken cancellationToken)
        {
            if (exception is not UnauthorizedException unauthorizedException) return false;

            _logger.LogError(unauthorizedException
                , "Un authorized exception has occured : {Message}", unauthorizedException.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = unauthorizedException.Message,
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized"
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
