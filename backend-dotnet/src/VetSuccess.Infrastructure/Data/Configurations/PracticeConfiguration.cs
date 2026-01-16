using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class PracticeConfiguration : IEntityTypeConfiguration<Practice>
{
    public void Configure(EntityTypeBuilder<Practice> builder)
    {
        builder.ToTable("apps_practice");
        
        builder.HasKey(p => p.Uuid);
        builder.Property(p => p.PracticeOduId).HasColumnName("PRACTICE_ODU_ID");
        
        builder.Property(p => p.Uuid).HasColumnName("uuid");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(p => p.DataSource).HasColumnName("data_source");
        
        builder.Property(p => p.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(p => p.PracticeName).HasColumnName("NAME").HasMaxLength(256);
        builder.Property(p => p.Address1).HasColumnName("ADDRESS_1");
        builder.Property(p => p.Address2).HasColumnName("ADDRESS_2");
        builder.Property(p => p.City).HasColumnName("CITY");
        builder.Property(p => p.State).HasColumnName("STATE");
        builder.Property(p => p.Country).HasColumnName("COUNTRY");
        builder.Property(p => p.ZipCode).HasColumnName("ZIP");
        builder.Property(p => p.Phone).HasColumnName("PHONE").HasMaxLength(256);
        builder.Property(p => p.HasPimsConnection).HasColumnName("HAS_PIMS_CONNECTION");
        builder.Property(p => p.Pims).HasColumnName("PIMS");
        builder.Property(p => p.LatestExtractorUpdated).HasColumnName("LATEST_EXTRACTOR_UPDATED_AT_UTC");
        builder.Property(p => p.LatestTransaction).HasColumnName("LATEST_TRANSACTION");
        builder.Property(p => p.ServerImportFinished).HasColumnName("SERVER_IMPORT_FINISHED_AT_UTC");
        builder.Property(p => p.PracticeUpdatedAt).HasColumnName("PRACTICE_UPDATED_AT_UTC");
        builder.Property(p => p.IsArchived).HasColumnName("APP_IS_ARCHIVED");
        
        builder.HasOne(p => p.Server)
            .WithMany(s => s.Practices)
            .HasForeignKey(p => p.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.PracticeSettings)
            .WithOne(s => s.Practice)
            .HasForeignKey<PracticeSettings>(s => s.PracticeOduId)
            .HasPrincipalKey<Practice>(p => p.PracticeOduId);
        
        builder.HasQueryFilter(p => p.ExtractorRemovedAt == null);
    }
}
