using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.ToTable("apps_server");
        
        builder.HasKey(s => s.Uuid);
        builder.Property(s => s.ServerOduId).HasColumnName("SERVER_ODU_ID");
        
        builder.Property(s => s.Uuid).HasColumnName("uuid");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(s => s.DataSource).HasColumnName("data_source");
        
        builder.Property(s => s.ServerName).HasColumnName("NAME");
        builder.Property(s => s.PimsVersion).HasColumnName("PIMS_VERSION");
        builder.Property(s => s.LatestExtractorUpdated).HasColumnName("LATEST_EXTRACTOR_UPDATED_AT_UTC");
        builder.Property(s => s.ServerImportFinished).HasColumnName("SERVER_IMPORT_FINISHED_AT_UTC");
        
        builder.HasQueryFilter(s => s.ExtractorRemovedAt == null);
    }
}
