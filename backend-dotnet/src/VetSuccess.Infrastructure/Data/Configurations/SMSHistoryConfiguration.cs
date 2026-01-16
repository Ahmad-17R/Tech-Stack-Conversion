using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class SMSHistoryConfiguration : IEntityTypeConfiguration<SMSHistory>
{
    public void Configure(EntityTypeBuilder<SMSHistory> builder)
    {
        builder.ToTable("apps_smshistory");
        
        builder.HasKey(s => s.Uuid);
        builder.Property(s => s.Uuid).HasColumnName("uuid");
        
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        
        builder.Property(s => s.ClientOduId).HasColumnName("client_id");
        builder.Property(s => s.PracticeOduId).HasColumnName("practice_id");
        builder.Property(s => s.Status).HasColumnName("status");
        builder.Property(s => s.Notes).HasColumnName("notes");
        builder.Property(s => s.SentAt).HasColumnName("sent_at");
        builder.Property(s => s.IsFollowed).HasColumnName("is_followed");
        builder.Property(s => s.MessageText).HasColumnName("message");
        builder.Property(s => s.PhoneNumber).HasColumnName("phone_number");
        
        builder.HasOne(s => s.Client)
            .WithMany(c => c.SMSHistories)
            .HasForeignKey(s => s.ClientOduId)
            .HasPrincipalKey(c => c.ClientOduId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(s => s.Practice)
            .WithMany(p => p.SMSHistories)
            .HasForeignKey(s => s.PracticeOduId)
            .HasPrincipalKey(p => p.PracticeOduId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
