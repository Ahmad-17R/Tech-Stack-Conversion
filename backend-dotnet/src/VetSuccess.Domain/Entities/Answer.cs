namespace VetSuccess.Domain.Entities;

public class Answer : BaseEntity
{
    public string PracticeOduId { get; set; } = null!;
    public Practice Practice { get; set; } = null!;
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    
    public string AnswerText { get; set; } = null!;
}
