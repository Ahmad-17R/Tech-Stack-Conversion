using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("apps_patient");
        
        builder.HasKey(p => p.Uuid);
        builder.Property(p => p.PatientOduId).HasColumnName("PATIENT_ODU_ID");
        
        // Base entity properties
        builder.Property(p => p.Uuid).HasColumnName("uuid");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
        builder.Property(p => p.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(p => p.DataSource).HasColumnName("data_source");
        
        // Patient properties
        builder.Property(p => p.ServerOduId).HasColumnName("SERVER_ODU_ID");
        builder.Property(p => p.BirthDate).HasColumnName("BIRTH_DATE");
        builder.Property(p => p.DeathDate).HasColumnName("DEATH_DATE");
        builder.Property(p => p.EuthanasiaDate).HasColumnName("EUTHANASIA_DATE");
        builder.Property(p => p.PimsEnteredDate).HasColumnName("PIMS_ENTERED_DATE");
        builder.Property(p => p.EarliestMedicalServiceDate).HasColumnName("EARLIEST_MEDICAL_SERVICE_DATE");
        builder.Property(p => p.PatientName).HasColumnName("NAME");
        builder.Property(p => p.PimsId).HasColumnName("PIMS_ID");
        builder.Property(p => p.Species).HasColumnName("SPECIES");
        builder.Property(p => p.SpeciesDescription).HasColumnName("SPECIES_DESCRIPTION");
        builder.Property(p => p.Breed).HasColumnName("BREED");
        builder.Property(p => p.BreedDescription).HasColumnName("BREED_DESCRIPTION");
        builder.Property(p => p.Color).HasColumnName("COLOR");
        builder.Property(p => p.ColorDescription).HasColumnName("COLOR_DESCRIPTION");
        builder.Property(p => p.Gender).HasColumnName("GENDER");
        builder.Property(p => p.GenderDescription).HasColumnName("GENDER_DESCRIPTION");
        builder.Property(p => p.Weight).HasColumnName("WEIGHT");
        builder.Property(p => p.WeightUnits).HasColumnName("WEIGHT_UNITS");
        builder.Property(p => p.PimsIsDeleted).HasColumnName("PIMS_IS_DELETED");
        builder.Property(p => p.OduIsDeleted).HasColumnName("ODU_IS_DELETED");
        builder.Property(p => p.PimsIsDeceased).HasColumnName("PIMS_IS_DECEASED");
        builder.Property(p => p.IsDeceased).HasColumnName("IS_DECEASED");
        builder.Property(p => p.PimsIsInactive).HasColumnName("PIMS_IS_INACTIVE");
        builder.Property(p => p.IsSafeToContact).HasColumnName("IS_SAFE_TO_CONTACT");
        builder.Property(p => p.PimsHasSuspendedReminders).HasColumnName("PIMS_HAS_SUSPENDED_REMINDERS");
        builder.Property(p => p.IsOnline).HasColumnName("IS_ONLINE");
        builder.Property(p => p.IsInclinic).HasColumnName("IS_INCLINIC");
        builder.Property(p => p.NewDateUpdatedAt).HasColumnName("IS_NEW_DATE_UPDATED_AT_UTC");
        builder.Property(p => p.LatestMedicalServiceDate).HasColumnName("LATEST_MEDICAL_SERVICE_DATE");
        builder.Property(p => p.PatientNewDate).HasColumnName("PATIENT_IS_NEW_DATE");
        builder.Property(p => p.PatientRecordUpdatedAt).HasColumnName("PATIENT_RECORD_UPDATED_AT_UTC");
        
        // Custom application fields
        builder.Property(p => p.OutcomeOduId).HasColumnName("APP_OUTCOME").HasMaxLength(255);
        builder.Property(p => p.OptOut).HasColumnName("APP_OPT_OUT");
        builder.Property(p => p.Comment).HasColumnName("APP_COMMENT").HasMaxLength(1000);
        builder.Property(p => p.OutcomeAt).HasColumnName("APP_OUTCOME_AT");
        
        // Relationships
        builder.HasOne(p => p.Server)
            .WithMany(s => s.Patients)
            .HasForeignKey(p => p.ServerOduId)
            .HasPrincipalKey(s => s.ServerOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Outcome)
            .WithMany(o => o.Patients)
            .HasForeignKey(p => p.OutcomeOduId)
            .HasPrincipalKey(o => o.OutcomeOduId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Query filter for soft delete
        builder.HasQueryFilter(p => p.ExtractorRemovedAt == null);
    }
}
