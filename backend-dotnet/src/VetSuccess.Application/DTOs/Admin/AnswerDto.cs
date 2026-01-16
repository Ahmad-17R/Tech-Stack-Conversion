namespace VetSuccess.Application.DTOs.Admin;

public class AnswerDto
{
    public Guid Uuid { get; set; }
    public string PracticeOduId { get; set; } = null!;
    public string? PracticeName { get; set; }
    public Guid QuestionId { get; set; }
    public string? QuestionText { get; set; }
    public string AnswerText { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAnswerRequest
{
    public string PracticeOduId { get; set; } = null!;
    public Guid QuestionId { get; set; }
    public string AnswerText { get; set; } = null!;
}

public class UpdateAnswerRequest
{
    public string? AnswerText { get; set; }
    public Guid? QuestionId { get; set; }
}
