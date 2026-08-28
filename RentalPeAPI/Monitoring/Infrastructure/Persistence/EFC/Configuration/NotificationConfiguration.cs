using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Notification.
/// Mapea correctamente las columnas de base de datos según el modelo DDD refactorizado.
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(n => n.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(n => n.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();
        
        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(n => n.Message)
            .HasColumnName("message")
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("idx_notifications_user_id");
        builder.HasIndex(n => n.SpaceId)
            .HasDatabaseName("idx_notifications_space_id");
        
        builder.HasIndex(n => new { n.UserId, n.IsRead })
            .HasDatabaseName("idx_notifications_user_unread");
    }
}
