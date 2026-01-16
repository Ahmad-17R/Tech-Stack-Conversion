namespace VetSuccess.Application.DTOs.Admin;

public class QuestionDto
{
    public Guid Uuid { get; set; }
    public string QuestionText { get; set; } = null!;
    public int? DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateQuestionRequest
{
    public string QuestionText { get; set; } = null!;
    public int? DisplayOrder { get; set; }
}

public class UpdateQuestionRequest
{
    public string? QuestionText { get; set; }
    public int? DisplayOrder { get; set; }
}
