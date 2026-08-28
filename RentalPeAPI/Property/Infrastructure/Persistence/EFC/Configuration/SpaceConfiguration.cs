using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.Property.Domain.Aggregates;
using RentalPeAPI.Property.Domain.Aggregates.Enums;

namespace RentalPeAPI.Property.Infrastructure.Persistence.EFC.Configuration;

public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("spaces");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.HomeownerId)
            .IsRequired();

        builder.Property(s => s.RemodelerId)
            .IsRequired(false);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(s => s.Location)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.SpaceType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.DimensionsSquareMeters)
            .IsRequired()
            .HasColumnType("decimal(10, 2)");

        builder.Property(s => s.EstimatedBudget)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(s => s.EndingPricing)
            .IsRequired()
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValue(0m);

        builder.Property(s => s.IsOverBudgetNotified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("PEN");

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(SpaceStatus.Published);

        builder.Property(s => s.HasIot)
            .IsRequired()
            .HasDefaultValue(false);

        var listComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList()
        );

        builder.Property(s => s.Images)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
            )
            .Metadata.SetValueComparer(listComparer);

        builder.Property(s => s.PublishedAt)
            .IsRequired();

        builder.Property(s => s.AcceptedAt)
            .IsRequired(false);
        
        builder.HasIndex(s => s.HomeownerId);
        builder.HasIndex(s => s.RemodelerId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => new { s.HomeownerId, s.Status });
    }
}
