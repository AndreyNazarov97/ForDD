
using Microsoft.OpenApi.Models;

namespace ForDD.API
{
    public static class Startup
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddApiVersioning()
                .AddApiExplorer(options =>
                {
                    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "ForDD.API",
                    Description = "Verion 1.0",
                    TermsOfService = new Uri("https://www.google.com"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Andrey Nazarov",
                        Email = "nazarov6218@yandex.ru"
                    }
                });

                options.SwaggerDoc("v2", new OpenApiInfo()
                {
                    Version = "v2",
                    Title = "ForDD.API",
                    Description = "Verion 2.0",
                    TermsOfService = new Uri("https://www.google.com"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Andrey Nazarov",
                        Email = "nazarov6218@yandex.ru"
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "enter valid token",
                    Name = "Autorization",
                    Type =   SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            
                        },
                        Array.Empty<string>()
                    }
                });
                
            });
        }

    }
}
