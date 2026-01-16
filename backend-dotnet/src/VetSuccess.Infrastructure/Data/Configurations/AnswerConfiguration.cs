using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetSuccess.Domain.Entities;

namespace VetSuccess.Infrastructure.Data.Configurations;

public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("apps_answer");
        
        builder.HasKey(a => a.Uuid);
        builder.Property(a => a.Uuid).HasColumnName("uuid");
        
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at");
        builder.Property(a => a.PracticeOduId).HasColumnName("practice_id").IsRequired();
        builder.Property(a => a.QuestionId).HasColumnName("question_id").IsRequired();
        builder.Property(a => a.AnswerText).HasColumnName("text").HasMaxLength(4000).IsRequired();
        
        builder.HasOne(a => a.Practice)
            .WithMany(p => p.Answers)
            .HasForeignKey(a => a.PracticeOduId)
            .HasPrincipalKey(p => p.PracticeOduId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(a => new { a.PracticeOduId, a.QuestionId })
            .IsUnique()
            .HasDatabaseName("unique_practice_question");
    }
}
