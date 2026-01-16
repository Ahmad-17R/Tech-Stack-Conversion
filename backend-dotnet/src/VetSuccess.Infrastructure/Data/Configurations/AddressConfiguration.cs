using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("apps_address");
        
        builder.HasKey(a => a.Uuid);
        builder.Property(a => a.AddressOduId).HasColumnName("ADDRESS_ODU_ID").HasMaxLength(255);
        
        builder.Property(a => a.Uuid).HasColumnName("uuid");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(a => a.DataSource).HasColumnName("data_source");
        
        builder.Property(a => a.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(a => a.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        builder.Property(a => a.AddressLine1).HasColumnName("ADDRESS_1");
        builder.Property(a => a.AddressLine2).HasColumnName("ADDRESS_2");
        builder.Property(a => a.City).HasColumnName("CITY");
        builder.Property(a => a.State).HasColumnName("STATE");
        builder.Property(a => a.PostalCode).HasColumnName("POSTAL_CODE");
        builder.Property(a => a.AddressType).HasColumnName("ADDRESS_TYPE");
        builder.Property(a => a.IsPreferred).HasColumnName("IS_PRIMARY");
        
        builder.HasOne(a => a.Server)
            .WithMany(s => s.Addresses)
            .HasForeignKey(a => a.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(a => a.Client)
            .WithMany(c => c.Addresses)
            .HasForeignKey(a => a.ClientOduId)
            .HasPrincipalKey(c => c.ClientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(a => a.ExtractorRemovedAt == null);
    }
}
