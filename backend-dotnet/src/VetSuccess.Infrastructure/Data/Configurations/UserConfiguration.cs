using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Use standard ASP.NET Identity table name
        builder.ToTable("AspNetUsers");
        
        // Keep custom properties with their column names
        builder.Property(u => u.Uuid).HasColumnName("Uuid");
        builder.Property(u => u.CreatedAt).HasColumnName("CreatedAt");
        builder.Property(u => u.UpdatedAt).HasColumnName("UpdatedAt");
        builder.Property(u => u.RefreshToken).HasColumnName("RefreshToken");
        builder.Property(u => u.RefreshTokenExpiryTime).HasColumnName("RefreshTokenExpiryTime");
        
        builder.HasIndex(u => u.Uuid).IsUnique();
    }
}
