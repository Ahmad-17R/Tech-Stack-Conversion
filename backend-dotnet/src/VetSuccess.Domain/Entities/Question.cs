namespace VetSuccess.Domain.Entities;

public class Question : BaseEntity
{
    public string QuestionText { get; set; } = null!;
    public int? DisplayOrder { get; set; }
    
    // Navigation properties
    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
