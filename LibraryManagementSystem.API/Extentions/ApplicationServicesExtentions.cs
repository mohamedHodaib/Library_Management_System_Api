using Asp.Versioning;
using AspNetCoreRateLimit;
using LibraryBookManagementSystem.API.Middlewares.ExceptionHandlers;
using LibraryManagementSystem.API.ExceptionHandlers;
using LibraryManagementSystem.API.Filters.ActionFilters;
using LibraryManagementSystem.Business.Contract;
using LibraryManagementSystem.Business.Options;
using LibraryManagementSystem.Business.Services;
using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            // Register Business Layer Injections & Services
            services.AddScoped<LogPerformanceFilterAttribute>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Register Api Layer Injections 
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IBorrowerService, BorrowerService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<SignInManager<User>>();
            
            //Register Exception Handlers
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<NotFoundExceptionHandler>();
            services.AddExceptionHandler<ConflictExceptionHandler>();
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<DefaultExceptionHandler>();

            //register problem details service
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = (context) =>
                {
                    // Add trace ID to all problem details responses
                    context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                    // Add timestamp
                    context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;

                    // Customize based on e@nvironment
                    if (!builder.Environment.IsDevelopment())
                    {
                        // In production, you might want to hide certain details
                        // Remove stack traces, etc.
                        context.ProblemDetails.Extensions.Remove("exception");
                    }

                    //add where the error occurred
                    context.ProblemDetails.Extensions["instance"] = 
                        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                };
            });

            // Register AutoMapper
            services.AddAutoMapper(typeof(BookService));

            // Bind Settings from appsettings.json
            services.Configure<LoanSettings>(builder.Configuration.GetSection(LoanSettings.SectionName));
            services.Configure<ForgotPasswordSettings>(builder.Configuration.GetSection(ForgotPasswordSettings.SectionName));
            services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));

            //Add Versioning Service
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });


            //Registerations to add Rate Limiting feature 
            //Add Memory Cache to rate limiting
            // 1. Add IOptions to read configuration
            builder.Services.AddOptions();

            // 2. Add in-memory cache to store rate limit counters
            builder.Services.AddMemoryCache();

            // 3. Bind the IP Rate Limiting options from configuration (e.g., appsettings.json)
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

            // 4. Inject the in-memory stores for counters and policies
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            //5. This registers the component that resolves the InvalidOperationException
            builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            // 6. Add the core Rate Limit Configuration
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // OPTIONAL but Recommended: Required for the rate limiter to find the HTTP context
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
