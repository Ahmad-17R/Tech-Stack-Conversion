using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("apps_appointment");
        
        builder.HasKey(a => a.Uuid);
        builder.Property(a => a.AppointmentOduId).HasColumnName("APPOINTMENT_ODU_ID");
        
        builder.Property(a => a.Uuid).HasColumnName("uuid");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(a => a.DataSource).HasColumnName("data_source");
        
        builder.Property(a => a.PatientOduId).HasColumnName("PATIENT_ODU_ID");
        builder.Property(a => a.AppointmentDate).HasColumnName("APPOINTMENT_DATETIME");
        builder.Property(a => a.AppointmentType).HasColumnName("APPOINTMENT_TYPE");
        builder.Property(a => a.Status).HasColumnName("STATUS");
        builder.Property(a => a.IsCanceledAppointment).HasColumnName("IS_CANCELED_APPOINTMENT");
        builder.Property(a => a.Description).HasColumnName("DESCRIPTION");
        
        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientOduId)
            .HasPrincipalKey(p => p.PatientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(a => a.ExtractorRemovedAt == null);
    }
}
