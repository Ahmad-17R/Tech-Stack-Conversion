namespace VetSuccess.Domain.Entities;

public class ClientPatientRelationship : BaseCallCenterEntity
{
    public string RelationshipOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    public string? ClientOduId { get; set; }
    public Client? Client { get; set; }
    public string? PatientOduId { get; set; }
    public Patient? Patient { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsPreferred { get; set; }
    public float? Percentage { get; set; }
    public string? RelationshipType { get; set; }
}
