namespace VetSuccess.Domain.Entities;

public class Server : BaseCallCenterEntity
{
    public string ServerOduId { get; set; } = null!;
    public string? ServerName { get; set; }
    public string? PimsVersion { get; set; }
    public DateTime? LatestExtractorUpdated { get; set; }
    public DateTime? ServerImportFinished { get; set; }
    
    // Navigation properties
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<Practice> Practices { get; set; } = new List<Practice>();
    public ICollection<Email> Emails { get; set; } = new List<Email>();
    public ICollection<Phone> Phones { get; set; } = new List<Phone>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<ClientPatientRelationship> Relationships { get; set; } = new List<ClientPatientRelationship>();
}
