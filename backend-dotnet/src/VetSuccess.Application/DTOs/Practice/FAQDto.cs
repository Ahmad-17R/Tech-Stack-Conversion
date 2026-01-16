namespace VetSuccess.Application.DTOs.Practice;

public class FAQDto
{
    public Guid Uuid { get; set; }
    public string? QuestionText { get; set; }
    public string? AnswerText { get; set; }
    public int? DisplayOrder { get; set; }
}
