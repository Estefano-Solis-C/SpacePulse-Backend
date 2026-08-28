using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPeAPI.User.Domain; 

namespace RentalPeAPI.User.Infrastructure.Persistence.EFC.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<Domain.User>
{
    public void Configure(EntityTypeBuilder<Domain.User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Email)
            .IsUnique(); 

        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasMaxLength(50);

        builder.Property(u => u.Photo)
            .HasMaxLength(250);
        
        builder.HasMany(u => u.PaymentMethods)
            .WithOne()
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}