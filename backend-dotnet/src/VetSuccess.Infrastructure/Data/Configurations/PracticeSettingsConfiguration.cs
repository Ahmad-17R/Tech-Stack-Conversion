using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class PracticeSettingsConfiguration : IEntityTypeConfiguration<PracticeSettings>
{
    public void Configure(EntityTypeBuilder<PracticeSettings> builder)
    {
        builder.ToTable("apps_practicesettings");
        
        builder.HasKey(ps => ps.Uuid);
        builder.Property(ps => ps.Uuid).HasColumnName("uuid");
        
        builder.Property(ps => ps.CreatedAt).HasColumnName("created_at");
        builder.Property(ps => ps.UpdatedAt).HasColumnName("updated_at");
        builder.Property(ps => ps.PracticeOduId).HasColumnName("practice_id").IsRequired();
        
        builder.Property(ps => ps.IsSmsMailingEnabled).HasColumnName("is_sms_mailing_enabled");
        builder.Property(ps => ps.IsEmailUpdatesEnabled).HasColumnName("is_email_updates_enabled");
        builder.Property(ps => ps.SmsSendersPhone).HasColumnName("sms_senders_phone").HasMaxLength(10);
        builder.Property(ps => ps.SmsScheduler).HasColumnName("sms_scheduler").HasMaxLength(100);
        builder.Property(ps => ps.SmsPracticeName).HasColumnName("sms_practice_name").HasMaxLength(100);
        builder.Property(ps => ps.SmsPhone).HasColumnName("sms_phone").HasMaxLength(20);
        builder.Property(ps => ps.SmsLink).HasColumnName("sms_link").HasMaxLength(150);
        builder.Property(ps => ps.Email).HasColumnName("email");
        builder.Property(ps => ps.LaunchDate).HasColumnName("launch_date");
        builder.Property(ps => ps.StartDateForLaunch).HasColumnName("start_date_for_launch");
        builder.Property(ps => ps.EndDateForLaunch).HasColumnName("end_date_for_launch");
        builder.Property(ps => ps.SchedulerEmail).HasColumnName("scheduler_email");
        builder.Property(ps => ps.RdoName).HasColumnName("rdo_name").HasMaxLength(100);
        builder.Property(ps => ps.RdoEmail).HasColumnName("rdo_email");
    }
}
