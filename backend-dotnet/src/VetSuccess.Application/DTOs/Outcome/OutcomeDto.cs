namespace VetSuccess.Application.DTOs.Outcome;

public class OutcomeDto
{
    public string OutcomeOduId { get; set; } = string.Empty;
    public Guid Uuid { get; set; }
    public string? OutcomeName { get; set; }
    public string? Description { get; set; }
    public bool RequiresFollowUp { get; set; }
    public DateTime CreatedAt { get; set; }
}
