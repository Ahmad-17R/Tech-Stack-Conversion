namespace VetSuccess.Domain.Entities;

public class Outcome : BaseEntity
{
    public string OutcomeOduId { get; set; } = null!;
    public string OutcomeName { get; set; } = null!;
    public string? Description { get; set; }
    public bool RequiresFollowUp { get; set; }
    
    // Navigation properties
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
