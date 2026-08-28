using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Monitoring.Domain.Model.Aggregates;

namespace RentalPeAPI.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public class IoTDeviceConfiguration : IEntityTypeConfiguration<IoTDevice>
{
    public void Configure(EntityTypeBuilder<IoTDevice> builder)
    {
        builder.ToTable("iot_devices");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.Property(d => d.SpaceId)
            .HasColumnName("space_id")
            .IsRequired();
        builder.HasIndex(d => d.SpaceId);
        builder.Property(d => d.CreatedByUserId)
            .HasColumnName("created_by_user_id")
            .IsRequired();
        builder.HasIndex(d => d.CreatedByUserId);
        builder.Property(d => d.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(d => d.SerialNumber)
            .HasColumnName("serial_number")
            .HasMaxLength(100)
            .IsRequired(false);
        builder.Property(d => d.MetricName)
            .HasColumnName("metric_name")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(d => d.Unit)
            .HasColumnName("unit")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(d => d.Value)
            .HasColumnName("value")
            .IsRequired();
        builder.Property(d => d.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
        builder.Property(d => d.IsOn)
            .HasColumnName("is_on")
            .IsRequired();
        builder.Property<decimal>("MinThreshold")
            .HasColumnName("min_threshold")
            .IsRequired();
        builder.Property<decimal>("MaxThreshold")
            .HasColumnName("max_threshold")
            .IsRequired();
        
        builder.Property<bool>("IsInAlertState")
            .HasColumnName("is_in_alert_state")
            .IsRequired()
            .HasDefaultValue(false);
    }
}