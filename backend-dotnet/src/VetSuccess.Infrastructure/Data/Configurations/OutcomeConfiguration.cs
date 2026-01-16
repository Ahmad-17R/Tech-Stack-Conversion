using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class OutcomeConfiguration : IEntityTypeConfiguration<Outcome>
{
    public void Configure(EntityTypeBuilder<Outcome> builder)
    {
        builder.ToTable("apps_outcome");
        
        builder.HasKey(o => o.Uuid);
        builder.Property(o => o.Uuid).HasColumnName("uuid");
        
        builder.Property(o => o.CreatedAt).HasColumnName("created_at");
        builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");
        builder.Property(o => o.OutcomeOduId).HasColumnName("outcome_odu_id").HasMaxLength(255).IsRequired();
        builder.Property(o => o.OutcomeName).HasColumnName("text").HasMaxLength(255).IsRequired();
        builder.Property(o => o.Description).HasColumnName("description");
        builder.Property(o => o.RequiresFollowUp).HasColumnName("requires_follow_up");
    }
}
