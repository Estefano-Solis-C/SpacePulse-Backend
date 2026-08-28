using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.User.Domain;

namespace RentalPeAPI.User.Infrastructure.Persistence.EFC.Configuration;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("UserPaymentMethods");

        builder.HasKey(pm => pm.Id);

        builder.Property(pm => pm.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(pm => pm.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pm => pm.LastFourDigits)
            .IsRequired()
            .HasMaxLength(4)
            .HasColumnName("last_four_digits");

        builder.Property(pm => pm.Expiry)
            .IsRequired()
            .HasMaxLength(10);
    }
}