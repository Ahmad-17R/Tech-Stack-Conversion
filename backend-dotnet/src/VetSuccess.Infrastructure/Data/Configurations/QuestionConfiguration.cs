using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("apps_question");
        
        builder.HasKey(q => q.Uuid);
        builder.Property(q => q.Uuid).HasColumnName("uuid");
        
        builder.Property(q => q.CreatedAt).HasColumnName("created_at");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at");
        builder.Property(q => q.QuestionText).HasColumnName("text").HasMaxLength(255).IsRequired();
        builder.Property(q => q.DisplayOrder).HasColumnName("display_order");
    }
}
