namespace VetSuccess.Application.DTOs.Admin;

public class OutcomeAdminDto
{
    public Guid Uuid { get; set; }
    public string OutcomeOduId { get; set; } = null!;
    public string OutcomeName { get; set; } = null!;
    public string? Description { get; set; }
    public bool RequiresFollowUp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateOutcomeRequest
{
    public string OutcomeOduId { get; set; } = null!;
    public string OutcomeName { get; set; } = null!;
    public string? Description { get; set; }
    public bool RequiresFollowUp { get; set; }
}

public class UpdateOutcomeRequest
{
    public string? OutcomeName { get; set; }
    public string? Description { get; set; }
    public bool? RequiresFollowUp { get; set; }
}
