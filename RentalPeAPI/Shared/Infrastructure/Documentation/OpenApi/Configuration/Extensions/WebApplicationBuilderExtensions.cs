using Microsoft.OpenApi.Models;

namespace RentalPeAPI.Shared.Infrastructure.Documentation.OpenApi.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddOpenApiConfigurationServices(this WebApplicationBuilder builder)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
    
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "ACME.LearningCenterPlatform.API",
                    Version = "v1",
                    Description = "ACME Learning Center Platform API",
                    TermsOfService = new Uri("https://acme-learning.com/tos"),
                    Contact = new OpenApiContact
                    {
                        Name = "ACME Studios",
                        Email = "contact@acme.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache 2.0",
                        Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
                    }
                });
            options.EnableAnnotations();
        });

    }
}