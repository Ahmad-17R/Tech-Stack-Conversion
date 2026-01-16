using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class EmailConfiguration : IEntityTypeConfiguration<Email>
{
    public void Configure(EntityTypeBuilder<Email> builder)
    {
        builder.ToTable("apps_email");
        
        builder.HasKey(e => e.Uuid);
        builder.Property(e => e.EmailOduId).HasColumnName("EMAIL_ODU_ID").HasMaxLength(255);
        
        builder.Property(e => e.Uuid).HasColumnName("uuid");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(e => e.DataSource).HasColumnName("data_source");
        
        builder.Property(e => e.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(e => e.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        builder.Property(e => e.EmailType).HasColumnName("EMAIL_TYPE");
        builder.Property(e => e.EmailAddress).HasColumnName("ADDRESS");
        builder.Property(e => e.IsPreferred).HasColumnName("IS_PRIMARY");
        
        builder.HasIndex(e => new { e.EmailAddress, e.IsPreferred, e.ExtractorRemovedAt })
            .HasDatabaseName("filter_by_email_idx");
        
        builder.HasOne(e => e.Server)
            .WithMany(s => s.Emails)
            .HasForeignKey(e => e.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Client)
            .WithMany(c => c.Emails)
            .HasForeignKey(e => e.ClientOduId)
            .HasPrincipalKey(c => c.ClientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(e => e.ExtractorRemovedAt == null);
    }
}
