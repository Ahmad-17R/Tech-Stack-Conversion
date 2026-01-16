using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class ClientPatientRelationshipConfiguration : IEntityTypeConfiguration<ClientPatientRelationship>
{
    public void Configure(EntityTypeBuilder<ClientPatientRelationship> builder)
    {
        builder.ToTable("apps_clientpatientrelationship");
        
        builder.HasKey(r => r.Uuid);
        builder.Property(r => r.RelationshipOduId).HasColumnName("CLIENT_PATIENT_RELATIONSHIP_ODU_ID").HasMaxLength(255);
        
        builder.Property(r => r.Uuid).HasColumnName("uuid");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(r => r.DataSource).HasColumnName("data_source");
        
        builder.Property(r => r.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(r => r.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        builder.Property(r => r.PatientOduId).HasColumnName("PATIENT_ODU_ID");
        builder.Property(r => r.StartDate).HasColumnName("START_DATE");
        builder.Property(r => r.EndDate).HasColumnName("END_DATE");
        builder.Property(r => r.IsPreferred).HasColumnName("PIMS_IS_PRIMARY");
        builder.Property(r => r.Percentage).HasColumnName("PERCENTAGE");
        builder.Property(r => r.RelationshipType).HasColumnName("RELATIONSHIP_TYPE");
        
        builder.HasOne(r => r.Server)
            .WithMany(s => s.Relationships)
            .HasForeignKey(r => r.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(r => r.ExtractorRemovedAt == null);
    }
}
