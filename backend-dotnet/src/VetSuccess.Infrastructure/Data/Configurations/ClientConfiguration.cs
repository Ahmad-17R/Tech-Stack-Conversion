using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("apps_client");
        
        builder.HasKey(c => c.Uuid);
        builder.Property(c => c.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        
        // Base entity properties
        builder.Property(c => c.Uuid).HasColumnName("uuid");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(c => c.DataSource).HasColumnName("data_source");
        
        // Client properties
        builder.Property(c => c.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(c => c.PimsEnteredDate).HasColumnName("PIMS_ENTERED_DATE");
        builder.Property(c => c.EarliestTransactionDate).HasColumnName("EARLIEST_TRANSACTION_DATE");
        builder.Property(c => c.EarliestOnlineTransactionDate).HasColumnName("EARLIEST_ONLINE_TRANSACTION_DATETIME");
        builder.Property(c => c.IsNewDate).HasColumnName("CLIENT_IS_NEW_DATE");
        builder.Property(c => c.OnlineAccountCreated).HasColumnName("ONLINE_ACCOUNT_CREATED_AT_UTC");
        builder.Property(c => c.PimsId).HasColumnName("PIMS_ID");
        builder.Property(c => c.PimsIsDeleted).HasColumnName("PIMS_IS_DELETED");
        builder.Property(c => c.PimsIsInactive).HasColumnName("PIMS_IS_INACTIVE");
        builder.Property(c => c.PimsHasSuspendedReminders).HasColumnName("PIMS_HAS_SUSPENDED_REMINDERS");
        builder.Property(c => c.FirstName).HasColumnName("CLIENT_FIRST_NAME").HasMaxLength(255);
        builder.Property(c => c.LastName).HasColumnName("CLIENT_LAST_NAME").HasMaxLength(255);
        builder.Property(c => c.FullName).HasColumnName("CLIENT_FULL_NAME").HasMaxLength(511);
        builder.Property(c => c.IsOnline).HasColumnName("IS_ONLINE");
        builder.Property(c => c.IsInclinic).HasColumnName("IS_INCLINIC");
        builder.Property(c => c.NewDateUpdatedAt).HasColumnName("IS_NEW_DATE_UPDATED_AT_UTC");
        builder.Property(c => c.LatestTransactionDate).HasColumnName("LATEST_TRANSACTION_DATE");
        builder.Property(c => c.ClientRecordUpdatedAt).HasColumnName("CLIENT_RECORD_UPDATED_AT_UTC");
        builder.Property(c => c.IsSafeContact).HasColumnName("IS_SAFE_TO_CONTACT");
        builder.Property(c => c.IsHomePractice).HasColumnName("IS_HOME_PRACTICE");
        
        // Indexes
        builder.HasIndex(c => c.FullName).HasDatabaseName("upper_full_name_idx");
        builder.HasIndex(c => c.ClientOduId).HasDatabaseName("upper_odu_id_idx");
        
        // Relationships
        builder.HasOne(c => c.Server)
            .WithMany(s => s.Clients)
            .HasForeignKey(c => c.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(c => c.Patients)
            .WithMany(p => p.Clients)
            .UsingEntity<ClientPatientRelationship>(
                j => j.HasOne(r => r.Patient).WithMany(p => p.ClientPatientRelationships).HasForeignKey(r => r.PatientOduId).HasPrincipalKey(p => p.PatientOduId),
                j => j.HasOne(r => r.Client).WithMany(c => c.ClientPatientRelationships).HasForeignKey(r => r.ClientOduId).HasPrincipalKey(c => c.ClientOduId)
            );
        
        // Query filter for soft delete
        builder.HasQueryFilter(c => c.ExtractorRemovedAt == null);
    }
}
