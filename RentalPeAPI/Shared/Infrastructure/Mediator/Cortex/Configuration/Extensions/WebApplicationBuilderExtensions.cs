using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using RentalPeAPI.Shared.Infrastructure.Mediator.Cortex.Configuration;

namespace ACME.LearningCenterPlatform.API.Shared.Infrastructure.Mediator.Cortex.Configuration.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void AddCortexConfigurationServices(this WebApplicationBuilder builder)
    {
        // Add Mediator Injection Configuration
        builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));

        // Add Cortex Mediator for Event Handling
        builder.Services.AddCortexMediator(
            configuration: builder.Configuration,
            handlerAssemblyMarkerTypes: [typeof(Program)], configure: options =>
            {
                options.AddOpenCommandPipelineBehavior(typeof(LoggingCommandBehavior<>));
                //options.AddDefaultBehaviors();
            });
        
    }
}