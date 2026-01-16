using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.ToTable("apps_reminder");
        
        builder.HasKey(r => r.Uuid);
        builder.Property(r => r.ReminderOduId).HasColumnName("REMINDER_ODU_ID");
        
        builder.Property(r => r.Uuid).HasColumnName("uuid");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.ExtractorRemovedAt).HasColumnName("extractor_removed_at");
        builder.Property(r => r.DataSource).HasColumnName("data_source");
        
        builder.Property(r => r.PatientOduId).HasColumnName("PATIENT_ODU_ID");
        builder.Property(r => r.ClientOduId).HasColumnName("CLIENT_ODU_ID");
        builder.Property(r => r.PracticeOduId).HasColumnName("PRACTICE_ODU_ID");
        builder.Property(r => r.SMSHistoryId).HasColumnName("sms_history_id");
        builder.Property(r => r.DateDue).HasColumnName("DATE_DUE");
        builder.Property(r => r.Description).HasColumnName("DESCRIPTION");
        builder.Property(r => r.SMSStatus).HasColumnName("SMS_STATUS");
        
        builder.HasOne(r => r.Patient)
            .WithMany(p => p.Reminders)
            .HasForeignKey(r => r.PatientOduId)
            .HasPrincipalKey(p => p.PatientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.SMSHistory)
            .WithMany(s => s.Reminders)
            .HasForeignKey(r => r.SMSHistoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(r => r.ExtractorRemovedAt == null);
    }
}
