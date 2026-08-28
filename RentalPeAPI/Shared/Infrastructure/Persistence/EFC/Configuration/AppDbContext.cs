using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using Microsoft.EntityFrameworkCore;
using RentalPeAPI.User.Domain; 
using RentalPeAPI.User.Infrastructure.Persistence.EFC.Configuration; 
using RentalPeAPI.Property.Domain.Aggregates; 
using RentalPeAPI.Property.Infrastructure.Persistence.EFC.Configuration; 
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

namespace RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration; 

/// <summary>
/// Represents the application's database context using Entity Framework Core.
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // --- DBSETS DEL BOUNDED CONTEXT USER ---
    public DbSet<User.Domain.User> Users { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = default!;

    // --- DBSETS DEL BOUNDED CONTEXT PROPERTY/SPACES ---
    public DbSet<Space> Spaces { get; set; }

    // --- DBSETS DEL BOUNDED CONTEXT MONITORING ---
    public DbSet<IoTDevice> IoTDevices { get; set; }
    public DbSet<WorkItem> Tasks { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Add the created and updated interceptor
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    /// <summary>
    /// Configures the model for the database context.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- APLICACIÓN DE CONFIGURACIONES (Bounded Contexts Activos) ---
        
        // 1. User BC Configuration
        builder.ApplyConfiguration(new UserConfiguration()); 
        builder.ApplyConfiguration(new PaymentMethodConfiguration());
        
        // 2. Space/Property BC Configuration
        builder.ApplyConfiguration(new SpaceConfiguration());

        // 3. Monitoring BC Configuration
        builder.ApplyConfiguration(new IoTDeviceConfiguration());
        builder.ApplyConfiguration(new WorkItemConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                {
                    property.SetCollation("utf8mb4_general_ci");
                }
            }
        }

        // Configuración compartida
        builder.UseSnakeCaseNamingConvention();
    }
}
