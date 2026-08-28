
using RentalPeAPI.Shared.Domain.Repositories;

using RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Repositories; 


namespace RentalPeAPI.Shared.Infrastructure.Interfaces.ASP.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddSharedContextServices(this WebApplicationBuilder builder)
    {
       
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}