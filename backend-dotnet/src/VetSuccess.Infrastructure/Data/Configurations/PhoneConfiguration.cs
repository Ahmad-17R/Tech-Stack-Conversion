using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
    public void Configure(EntityTypeBuilder<Phone> builder)
    {
        builder.ToTable("apps_phone");
        
        builder.HasKey(p => p.Uuid);
        builder.Property(p => p.PhoneOduId).HasColumnName("PHONE_ODU_ID").HasMaxLength(255);
        
        builder.Property(p => p.Uuid).HasColumnName("uuid");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(p => p.DataSource).HasColumnName("data_source");
        
        builder.Property(p => p.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(p => p.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        builder.Property(p => p.PhoneNumber).HasColumnName("NUMBER");
        builder.Property(p => p.PhoneType).HasColumnName("PHONE_TYPE");
        builder.Property(p => p.IsPreferred).HasColumnName("IS_PRIMARY");
        builder.Property(p => p.AppNumber).HasColumnName("APP_NUMBER").HasMaxLength(10);
        
        builder.HasIndex(p => new { p.AppNumber, p.IsPreferred, p.ExtractorRemovedAt })
            .HasDatabaseName("filter_by_phone_idx");
        
        builder.HasOne(p => p.Server)
            .WithMany(s => s.Phones)
            .HasForeignKey(p => p.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Client)
            .WithMany(c => c.Phones)
            .HasForeignKey(p => p.ClientOduId)
            .HasPrincipalKey(c => c.ClientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(p => p.ExtractorRemovedAt == null);
    }
}
