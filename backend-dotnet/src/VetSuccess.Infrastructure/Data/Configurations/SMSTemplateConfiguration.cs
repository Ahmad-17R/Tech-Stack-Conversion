using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class SMSTemplateConfiguration : IEntityTypeConfiguration<SMSTemplate>
{
    public void Configure(EntityTypeBuilder<SMSTemplate> builder)
    {
        builder.ToTable("sms_templates");

        builder.HasKey(e => e.Uuid);

        builder.Property(e => e.Uuid)
            .HasColumnName("uuid")
            .IsRequired();

        builder.Property(e => e.Keywords)
            .HasColumnName("keywords")
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(e => e.Keywords)
            .IsUnique();

        builder.Property(e => e.Template)
            .HasColumnName("template")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
