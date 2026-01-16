using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class SMSEventConfiguration : IEntityTypeConfiguration<SMSEvent>
{
    public void Configure(EntityTypeBuilder<SMSEvent> builder)
    {
        builder.ToTable("sms_events");

        builder.HasKey(e => e.Uuid);

        builder.Property(e => e.Uuid)
            .HasColumnName("uuid")
            .IsRequired();

        builder.Property(e => e.SendAt)
            .HasColumnName("send_at")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Context)
            .HasColumnName("context")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
