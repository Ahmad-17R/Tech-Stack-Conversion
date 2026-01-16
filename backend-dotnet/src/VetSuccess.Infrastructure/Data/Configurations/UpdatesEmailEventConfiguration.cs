using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class UpdatesEmailEventConfiguration : IEntityTypeConfiguration<UpdatesEmailEvent>
{
    public void Configure(EntityTypeBuilder<UpdatesEmailEvent> builder)
    {
        builder.ToTable("updates_email_events");

        builder.HasKey(e => e.Uuid);

        builder.Property(e => e.Uuid)
            .HasColumnName("uuid")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.PracticeId)
            .HasColumnName("practice_id")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.FilePaths)
            .HasColumnName("file_paths")
            .HasColumnType("jsonb");

        builder.Property(e => e.ErrorMessage)
            .HasColumnName("error_message");

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasOne(e => e.Practice)
            .WithMany()
            .HasForeignKey(e => e.PracticeId)
            .HasPrincipalKey(p => p.PracticeOduId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
