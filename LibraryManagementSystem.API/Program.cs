using API.Extensions; // Import the extensions namespace
using LibraryBookManagementSystem.API.Middlewares;
using LibraryManagementSystem.API.Extensions;
using LibraryManagementSystem.API.Extentions;
//For Applying [ApiController] to all controllers in the assembly
using Microsoft.AspNetCore.Mvc;
[assembly: ApiController]

namespace LibraryManagementSystem.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Add services to the container using extension methods ---
            builder.AddSerilogLogging();

            builder.Services
                .AddApplicationServices(builder)
                .AddApiServices()
                .AddDatabaseServices(builder.Configuration)
                .AddIdentityAndAuthentication(builder.Configuration);

            var app = builder.Build();

            // --- Configure the HTTP request pipeline ---
            // ORDER MATTERS! The middleware pipeline executes in the order registered

            // 1. Exception handling should be first to catch all exceptions
            app.UseExceptionHandler(); // This will use ProblemDetails

            // 2. HSTS for production
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            // 3. HTTPS redirection
            app.UseHttpsRedirection();

            // 4. Status Code Pages - Important for handling non-success status codes
            // For APIs, we want to return problem details for error status codes
            app.UseStatusCodePages(async statusCodeContext =>
            {
                var context = statusCodeContext.HttpContext;

                // Don't interfere with responses that already have content
                if (context.Response.HasStarted || context.Response.ContentLength.HasValue)
                {
                    return;
                }

                var problemDetailsService = context.RequestServices.GetService<IProblemDetailsService>();
                if (problemDetailsService != null)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Status = context.Response.StatusCode,
                        Title = GetDefaultTitleForStatusCode(context.Response.StatusCode),
                        Type = GetDefaultTypeForStatusCode(context.Response.StatusCode),
                        Instance = context.Request.Path
                    };

                    await problemDetailsService.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = problemDetails
                    });
                }
            });

            // 5. Swagger (Development only)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1")
                );
            }

            // 6. Request logging middleware (before auth to log all requests)
            app.UseMiddleware<RequestLoggingHandler>();

            // 7. CORS
            app.UseCors("CorsPolicy");

            // 8. Response caching
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            // 9. Authentication & Authorization (must be in this order)
            app.UseAuthentication();
            app.UseAuthorization();

            // 10. Map controllers
            app.MapControllers();

            // Seed the database after middleware configuration
            await app.SeedDatabaseAsync();

            app.Run();
        }

        private static string GetDefaultTitleForStatusCode(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            405 => "Method Not Allowed",
            406 => "Not Acceptable",
            409 => "Conflict",
            415 => "Unsupported Media Type",
            422 => "Unprocessable Entity",
            429 => "Too Many Requests",
            500 => "Internal Server Error",
            501 => "Not Implemented",
            502 => "Bad Gateway",
            503 => "Service Unavailable",
            504 => "Gateway Timeout",
            _ => "An error occurred"
        };

        private static string GetDefaultTypeForStatusCode(int statusCode) => statusCode switch
        {
            400 => "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
            401 => "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized",
            403 => "https://datatracker.ietf.org/doc/html/rfc9110#name-403-forbidden",
            404 => "https://datatracker.ietf.org/doc/html/rfc9110#name-404-not-found",
            405 => "https://datatracker.ietf.org/doc/html/rfc9110#name-405-method-not-allowed",
            406 => "https://datatracker.ietf.org/doc/html/rfc9110#name-406-not-acceptable",
            409 => "https://datatracker.ietf.org/doc/html/rfc9110#name-409-conflict",
            415 => "https://datatracker.ietf.org/doc/html/rfc9110#name-415-unsupported-media-type",
            422 => "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2",
            429 => "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
            500 => "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error",
            501 => "https://datatracker.ietf.org/doc/html/rfc9110#name-501-not-implemented",
            502 => "https://datatracker.ietf.org/doc/html/rfc9110#name-502-bad-gateway",
            503 => "https://datatracker.ietf.org/doc/html/rfc9110#name-503-service-unavailable",
            504 => "https://datatracker.ietf.org/doc/html/rfc9110#name-504-gateway-timeout",
            _ => "https://datatracker.ietf.org/doc/html/rfc9110"
        };
    }
}