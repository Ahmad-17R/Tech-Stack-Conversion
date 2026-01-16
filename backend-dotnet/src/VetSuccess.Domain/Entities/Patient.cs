namespace VetSuccess.Domain.Entities;

public class Patient : BaseCallCenterEntity
{
    public string PatientOduId { get; set; } = null!;
    public string? ServerOduId { get; set; }
    public Server? Server { get; set; }
    
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public DateTime? EuthanasiaDate { get; set; }
    public DateTime? PimsEnteredDate { get; set; }
    public DateTime? EarliestMedicalServiceDate { get; set; }
    
    public string? PatientName { get; set; }
    public string? PimsId { get; set; }
    public string? Species { get; set; }
    public string? SpeciesDescription { get; set; }
    public string? Breed { get; set; }
    public string? BreedDescription { get; set; }
    public string? Color { get; set; }
    public string? ColorDescription { get; set; }
    public string? Gender { get; set; }
    public string? GenderDescription { get; set; }
    public string? Weight { get; set; }
    public string? WeightUnits { get; set; }
    
    public bool? PimsIsDeleted { get; set; }
    public string? OduIsDeleted { get; set; }
    public bool? PimsIsDeceased { get; set; }
    public bool? IsDeceased { get; set; }
    public bool? PimsIsInactive { get; set; }
    public bool? IsSafeToContact { get; set; }
    public bool? PimsHasSuspendedReminders { get; set; }
    
    public bool? IsOnline { get; set; }
    public bool? IsInclinic { get; set; }
    public DateTime? NewDateUpdatedAt { get; set; }
    public DateTime? LatestMedicalServiceDate { get; set; }
    public DateTime? PatientNewDate { get; set; }
    public DateTime? PatientRecordUpdatedAt { get; set; }
    
    // Custom application fields
    public string? OutcomeOduId { get; set; }
    public Outcome? Outcome { get; set; }
    public bool? OptOut { get; set; }
    public string? Comment { get; set; }
    public DateTime? OutcomeAt { get; set; }
    
    // Navigation properties
    public ICollection<ClientPatientRelationship> ClientPatientRelationships { get; set; } = new List<ClientPatientRelationship>();
    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
