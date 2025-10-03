using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;
using LibraryManagementSystem.Business.Options;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities.People;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagementSystem.API.Extensions
{
    public static class IdentityAndAuthExtensions
    {
        public static IServiceCollection AddIdentityAndAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Register User Identity
            services.AddIdentity<User, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                // Email settings
                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            var jwtKey = configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = HandleChallenge,
                    OnAuthenticationFailed = HandleAuthenticationFailed,
                    OnForbidden = HandleForbidden
                };
            });


            return services;
        }

        #region Helper



        //Called when authentication Failed
        private static async Task HandleAuthenticationFailed(AuthenticationFailedContext context)
        {
            context.HttpContext.Items["AuthenticationException"] = context.Exception;
            await Task.CompletedTask;
        }

        private static async Task HandleChallenge(JwtBearerChallengeContext context)
        {
            //prevent the default behaviour 
            context.HandleResponse();

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            string errorDetail;
            string errorType = "invalid_token";

            if (context.HttpContext.Items["AuthenticationException"] is Exception ex)
            {
                if (ex is SecurityTokenExpiredException)
                {
                    errorDetail = "The access token has expired. Please refresh your token or re-authenticate.";
                }
                else if (ex is SecurityTokenInvalidSignatureException)
                {
                    errorDetail = "The access token has an invalid signature.";
                }
                else
                {
                    errorDetail = "Authentication failed because the provided token is invalid.";
                }
            }
            else
            {
                errorType = "Credential_missing";
                errorDetail = "Authentication Credentials was not provided. please provide Jwt token in the authorization header.";
            }

            context.Response.Headers.Append("WWW-Authenticate", $"Bearer error=\"{errorType}\", error_description=\"{errorDetail}\"");

            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = errorDetail,
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-401-unauthorized",
            };

            await WriteProblemDetailsResponse(context.HttpContext, problemDetails);

        }

        private static async Task HandleForbidden(ForbiddenContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

            var errorDetail = "You do not have the necessary permissions to access this resource.";

            var problemDetails = new ProblemDetails
            {
                Title = "Forbidden",
                Status = (int)HttpStatusCode.Forbidden,
                Detail = errorDetail,
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-403-forbidden",
            };

            await WriteProblemDetailsResponse(context.HttpContext, problemDetails);

        }

        private static async Task WriteProblemDetailsResponse(HttpContext httpContext,ProblemDetails problemDetails)
        {
            // Try to use IProblemDetailsService if available
            var problemDetailsService = httpContext.RequestServices.GetService<IProblemDetailsService>();

            if (problemDetailsService != null)
            {
                await problemDetailsService.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });
            }
            else
            {
                // Fallback if IProblemDetailsService is not registered
                httpContext.Response.ContentType = "application/problem+json";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var json = JsonSerializer.Serialize(problemDetails, options);
                await httpContext.Response.WriteAsync(json);
            }
        }

        #endregion

    }
}
