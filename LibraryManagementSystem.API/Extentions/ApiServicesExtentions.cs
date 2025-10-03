using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Marvin.Cache.Headers;
using LibraryManagementSystem.API.Filters.ActionFilters;
using LibraryManagementSystem.API.Filters.ResultFilters;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.API.Extensions
{
    public static class ApiServicesExtensions
    {

        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
            new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IOptionsMonitor<MvcOptions>>()
            .CurrentValue
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();


            services.AddControllers(options =>
            {
                // Add Filters in Global Scope
                //Action filter
                options.Filters.Add<LogPerformanceFilterAttribute>();

                //Result Filter for HandlingPaged Result
                options.Filters.Add<HandlePagedDataFilterAttribute>(order:0);

                // Configure content negotiation settings
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;

                //Adding json patch input formating
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());

            })
             .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
             }); 

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Library Api", Version = "v1" ,
                    Description = "Library API To manage small Library business.",
					TermsOfService = new Uri("https://example.com/terms"),
					Contact = new OpenApiContact
					{
						Name = "Mohamed Hodaib",
						Email = "mohamedhegazi293@gmail.com",
						Url = new Uri("https://www.linkedin.com/in/mohamed-hodaib-2670b2344/"),
					},
					License = new OpenApiLicense
					{
						Name = "MIT License",
						Url = new Uri("https://opensource.org/licenses/MIT")
					}
				});

                //To enable XML Comments
				var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);

				//add authorization support
				s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					In = ParameterLocation.Header,
					Description = "Place to add JWT with Bearer",
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer"
				});
				s.AddSecurityRequirement(new OpenApiSecurityRequirement() 
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
					        },
                            Name = "Bearer",
                        },
                            new List<string>()
                    }
                });
			});

            // Add cores to allow access to the application
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                       builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .WithExposedHeaders("X-Pagination")
                );
            });

            //Register the services for caching
            services.AddResponseCaching();
            // 1. Add the core services for caching
            services.AddHttpCacheHeaders(
                (expirationOptions) =>
                {
                    expirationOptions.MaxAge = 60;
                },
                (validationOptions) =>
                {
                    validationOptions.MustRevalidate = true;
                }
            );

          

            return services;
        }
    }
}
