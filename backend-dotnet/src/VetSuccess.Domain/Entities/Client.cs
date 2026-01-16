namespace VetSuccess.Domain.Entities;

public class Client : BaseCallCenterEntity
{
    public string ClientOduId { get; set; } = null!;
    public string? PracticeOduId { get; set; }
    public Practice? Practice { get; set; }
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    
    public DateTime? PimsEnteredDate { get; set; }
    public DateTime? EarliestTransactionDate { get; set; }
    public DateTime? EarliestOnlineTransactionDate { get; set; }
    public DateTime? IsNewDate { get; set; }
    public DateTime? OnlineAccountCreated { get; set; }
    
    public string? PimsId { get; set; }
    public bool? PimsIsDeleted { get; set; }
    public bool? PimsIsInactive { get; set; }
    public bool? PimsHasSuspendedReminders { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? UpperFullName { get; set; }
    
    public bool? IsOnline { get; set; }
    public bool? IsInclinic { get; set; }
    public DateTime? NewDateUpdatedAt { get; set; }
    public DateTime? LatestTransactionDate { get; set; }
    public DateTime? ClientRecordUpdatedAt { get; set; }
    public bool? IsSafeContact { get; set; }
    public bool? IsHomePractice { get; set; }
    
    // Navigation properties
    public ICollection<Email> Emails { get; set; } = new List<Email>();
    public ICollection<Phone> Phones { get; set; } = new List<Phone>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<ClientPatientRelationship> ClientPatientRelationships { get; set; } = new List<ClientPatientRelationship>();
    public ICollection<Patient> Patients { get; set; } = new List<Patient>();
    public ICollection<SMSHistory> SMSHistories { get; set; } = new List<SMSHistory>();
}
